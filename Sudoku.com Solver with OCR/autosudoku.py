import cv2
import numpy as np
import pytesseract
import os
from pynput.mouse import Controller
from PIL import ImageGrab
import keyboard
import time
import solver
import GUI

pytesseract.pytesseract.tesseract_cmd = r'C:\Program Files\Tesseract-OCR\tesseract.exe'

    
def extractData():
    image = cv2.imread("screenshot.png")
    gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

    _, thresh = cv2.threshold(gray, 100, 255, cv2.THRESH_BINARY)

    blur = cv2.GaussianBlur(thresh, (5,5), 0)
    
    sudoku_grid = cv2.resize(blur, (396, 396))  # Resize to a fixed grid size

    cell_size = 44

    sudoku_numbers = ""
    custom_config = r'--psm 10 -c tessedit_char_whitelist=0123456789'

    border = 6

    # Loop through each of the 81 cells (9x9 grid)
    for row in range(9):
        for col in range(9):
            # Extract the cell from the grid
            cell = sudoku_grid[(row*cell_size)+border:((row+1)*cell_size)-border,
                    (col*cell_size)+border:((col+1)*cell_size)-border]
            
            # Check if cell is empty
            if np.all(cell == 255):
                digit = '0'
            else:
                # Apply OCR to the cell
                digit = pytesseract.image_to_string(cell, config=custom_config)
                digit = digit.strip()  # Clean up the extracted text
            
            # Append the digit (or '0' if not a valid digit) to the string
            sudoku_numbers += digit if digit.isdigit() else '0'

        GUI.updateprogress(((row+1)*10) + 5)

    return sudoku_numbers



def findsudokucorner():
    scale = 1.25
    
    mouse = Controller()
    mx,my = mouse.position

    size = 50
    x = mx * scale
    y = my * scale
    
    region = (x-size, y-size, x+size, y+size)
    screenshot = ImageGrab.grab(bbox=region)
    screenshot.save('one.png')
    
    img = cv2.imread("one.png", cv2.IMREAD_GRAYSCALE)
    binary = cv2.adaptiveThreshold(img, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, 
                              cv2.THRESH_BINARY_INV, 11, 75)
    contours, _ = cv2.findContours(binary, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    min_area = 50
    filtered_contours = [cnt for cnt in contours if cv2.contourArea(cnt) > min_area]

    try:
        top_left_contour = min(filtered_contours, key=lambda cnt: cv2.boundingRect(cnt)[0] + cv2.boundingRect(cnt)[1])
        x, y, w, h = cv2.boundingRect(top_left_contour)

        top_left_corner = (x, y)
        
        x = mx + (-(50 - x) / scale)
        y = my + (-(50 - y) / scale)

        os.remove("one.png")
        return x,y
    except ValueError:
        GUI.createerrormessage("The mouse was not on the puzzle")
        return None,None




def getscreenshot(x,y):
    scale = 1.25
    dimension = round(426 * scale)
    x*=scale
    y*=scale

    region = (x,y, x+dimension, y+dimension)
    time.sleep(0.5)
    screenshot = ImageGrab.grab(bbox=region)
    screenshot.save('screenshot.png')
    
    time.sleep(1)
    GUI.updateprogress(5)



def enterInput(solved):
    for i in range(0,9):
        for j in range(0,8):
            keyboard.send(solved[(i*9)+j])
            keyboard.send('right')

        keyboard.send(solved[(i*9)+8])
        if(i != 8):
            keyboard.send('down')
            for j in range(0,8):
                keyboard.send('left')

def main():
    GUI.starting()
    x,y = findsudokucorner()
    if x is not None:
        getscreenshot(x,y)
        data = extractData()
        solved = solver.StartSolver(data)
        GUI.finished()
        if '0' in solved:
            GUI.createerrormessage("The solver could'nt do this one :(")
        else:
            enterInput(solved)


    
keyboard.add_hotkey('shift+n', main)
GUI.GUI()
keyboard.wait()

