from pynput.mouse import Controller
import time
import tkinter as tk
from tkinter import ttk
import threading

interval = 5

mouse = Controller()

enabled = False

start_event = threading.Event()


def jiggle():


    for i in range(5):
        currX, currY = mouse.position

        mouse.position = currX + 15, currY - 15
        time.sleep(0.001)

    xamount = - 15
    yamount = 15
    for i in range(7):
        for j in range(10):
            currX, currY = mouse.position

            mouse.position = currX + xamount, currY + yamount
            time.sleep(0.001)

        xamount *= -1
        yamount *= -1


    for i in range(5):
        currX, currY = mouse.position

        mouse.position = currX + 15, currY - 15
        time.sleep(0.001)



def apply():
    global interval
    interval = int(timedropdown.get()[0:2])
    stop()
    


def mainLoop():
    global interval

    #print(interval)
    start_event.wait()
    for i in range(interval*60):

        if not start_event.is_set():
            break
        time.sleep(1)

    if start_event.is_set():
        jiggle()

    mainLoop()


def stop():
    start_event.clear()
    disableButton.config(state=(tk.DISABLED))
    enableButton.config(state=(tk.NORMAL))

def start():
    start_event.set()
    enableButton.config(state=(tk.DISABLED))
    disableButton.config(state=(tk.NORMAL))


def GUI():

    global timedropdown
    global enableButton
    global disableButton
    global root

    colour1 = "#EEEEEE"
    colour2 = "#DDDDDD"
    
    root = tk.Tk()
    root.title("Mouse Jiggle")
    root.geometry("300x250")
    root.resizable(False, False)
    root.configure(bg=colour1)

    frame = tk.Frame(root, bd=2, relief="solid")
    frame.place(x=10, y=20, width=280, height=100)
    
    subtitle = tk.Label(root, text="Options", bg=colour1, font=("Helvetica", 11))
    subtitle.place(x=30, y=10)

    tk.Label(frame, text="Interval Time:", font=("Helvetica", 10)).place(x=10,y=30)

    timeoption = tk.StringVar()
    timeoption.set("5 minutes")
    timeoptions = ["1 minute","2 minutes","3 minutes","5 minutes","10 minutes","15 minutes","30 minutes"]
    timedropdown = ttk.Combobox(frame, textvariable=timeoption, values=timeoptions, state="readonly", width=12)
    timedropdown.place(x=140, y=30)

    applyButton = tk.Button(frame, text="Apply", bg=colour2, font=("Helvetica", 10), width=8, command=apply)
    applyButton.place(x=150,y=60)
    

    enableButton = tk.Button(root, text="Enable", bg=colour2, font=("Helvetica", 12), width=14, height=4, command=start)
    enableButton.place(x=10,y=140)

    disableButton = tk.Button(root, text="Disable", bg=colour2, font=("Helvetica", 12), width=14, height=4, command=stop, state=(tk.DISABLED))
    disableButton.place(x=155,y=140)

    root.mainloop()



t1 = threading.Thread(target=mainLoop, name='t1', daemon=True)
t2 = threading.Thread(target=GUI, name='t2', daemon=True)

t1.start()
t2.start()

t2.join()
    
