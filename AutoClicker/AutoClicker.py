from pynput.mouse import Controller, Button
import threading
import time
import keyboard
import GUI


start_event = threading.Event()

mouse = Controller()

#Default values
cps = 10
interval = (1 / cps)

clickType = 0
mouseButton = Button.left


def updateValues(cps, clicktype, mousebutton):
    global interval
    global clickType
    global mouseButton
    
    interval = (1/int(cps))

    if clicktype == "Single":
        clickType = 0
    else:
        clickType = 1

    #print("updated")

    if mousebutton == "Left Click":
        mouseButton = Button.left;
    elif mousebutton == "Right Click":
        mouseButton = Button.right;
    else:
        mouseButton = Button.middle;
    
    
    

def clickLoop():
    global interval
    global clickType
    global mouseButton
    
    next_click = time.perf_counter()

    while True:

        start_event.wait()  # Wait if paused

        if clickType == 0:
            start_time = time.perf_counter()  # Start time for click
            mouse.click(mouseButton)  # Perform the click
            elapsed_time = time.perf_counter() - start_time  # Measure click time

            # Calculate the time for the next click
            next_click = max(next_click + interval, time.perf_counter())  # Prevent going backwards
            sleep_time = next_click - time.perf_counter() - elapsed_time  # Adjust sleep time to compensate for click duration

            if sleep_time > 0:
                time.sleep(sleep_time)  # Sleep only for the required time
            else:
                next_click = time.perf_counter()  # Reset if we're running behind
        else:
            mouse.press(mouseButton)
            time.sleep(0.05)
            

def toggle_autoclicker():
    global mouseButton
    if start_event.is_set():
        #print("Stopping autoclicker")
        mouse.release(mouseButton)
        start_event.clear()
        GUI.changebuttons2()
    else:
        #print("Starting autoclicker")
        start_event.set()
        GUI.changebuttons1()


GUI.setFunctions(updateValues, toggle_autoclicker)


keyboard.add_hotkey('shift+b', toggle_autoclicker)
t1 = threading.Thread(target=clickLoop, name='t1', daemon=True)
t2 = threading.Thread(target=GUI.setup, name='t2', daemon=True)

t2.start()
t1.start()

#t1.join()
t2.join()

    
