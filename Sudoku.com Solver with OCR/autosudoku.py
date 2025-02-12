import cv2
import numpy as np
import pytesseract
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
            
            # Apply OCR to the cell
            digit = pytesseract.image_to_string(cell, config=custom_config)
            digit = digit.strip()  # Clean up the extracted text
            
            # Append the digit (or '0' if not a valid digit) to the string
            sudoku_numbers += digit if digit.isdigit() else '0'

        GUI.updateprogress(((row+1)*10) + 5)

    return sudoku_numbers



def getscreenshot():
    scale = 1.25
    
    dimension = 426 * scale
    mouse = Controller()

    x,y = mouse.position

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
    getscreenshot()
    data = extractData()
    solved = solver.StartSolver(data)
    GUI.finished()
    if '0' in solved:
        GUI.createerrormessage()
    else:
        enterInput(solved)

keyboard.add_hotkey('shift+n', main)
GUI.GUI()
keyboard.wait()

