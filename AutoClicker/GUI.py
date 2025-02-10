import tkinter as tk
from tkinter import ttk


# Placeholder for function references
updateValuesFunc = None 
toggleFunc = None


def setFunctions(func1, func2):
    global updateValuesFunc
    global toggleFunc
    updateValuesFunc = func1
    toggleFunc = func2


def apply():
    global cps
    try:
        temp = int(cpsEntry.get())
        if(temp > 0 and temp < 51):
            cps = cpsEntry.get()
        else:
            print("Invalid entry")
    except:
        #print("Invalid entry")
        pass
        
    mouseButton = mousedropdown.get()
    clickType = clickdropdown.get()

    if updateValuesFunc:
        updateValuesFunc(cps,clickType, mouseButton)


def start():
    startbutton.config(state=tk.DISABLED)
    stopbutton.config(state=tk.NORMAL)
    if toggleFunc:
        toggleFunc()
    else:
        print("System Error")


def stop():
    stopbutton.config(state=tk.DISABLED)
    startbutton.config(state=tk.NORMAL)
    if toggleFunc:
        toggleFunc()
    else:
        print("System Error")


def changebuttons1():
    startbutton.config(state=tk.DISABLED)
    stopbutton.config(state=tk.NORMAL)

def changebuttons2():
    stopbutton.config(state=tk.DISABLED)
    startbutton.config(state=tk.NORMAL)



def setup():
    
    global cpsEntry
    global mousedropdown
    global clickdropdown
    global startbutton
    global stopbutton


    
    root = tk.Tk()
    root.title("AutoClicker")
    root.geometry("500x325")
    root.resizable(False, False)
    root.configure(bg=colour1)


    # Create a frame with a border
    frame1 = tk.Frame(root, bd=2, relief="solid")
    frame1.place(x=10, y=20, width=235, height=100)

    frame2 = tk.Frame(root, bd=2, relief="solid")
    frame2.place(x=255, y=20, width=235, height=100)

    # Create the title label overlapping the border
    subtitle1 = tk.Label(root, text="CPS", bg=colour1, font=("Helvetica", 11))
    subtitle1.place(x=30, y=10)  # Adjust to fit over the border break

    subtitle2 = tk.Label(root, text="Options", bg=colour1, font=("Helvetica", 11))
    subtitle2.place(x=275, y=10)  # Adjust to fit over the border break


    tk.Label(frame1, text="Clicks Per Second (CPS):", bg=colour1, font=("Helvetica", 10)).place(x=5,y=20)
    cpsEntry = tk.Entry(frame1, width=5)
    cpsEntry.place(x=180,y=20)


    tk.Label(frame2, text="Mouse Button:", bg=colour1, font=("Helvetica", 10)).place(x=5,y=20)
    tk.Label(frame2, text="Click Type:", bg=colour1, font=("Helvetica", 10)).place(x=5,y=50)

    mouseoption = tk.StringVar()
    mouseoption.set("Left Click")
    mouseoptions = ["Left Click","Right Click","Middle Mouse"]
    mousedropdown = ttk.Combobox(frame2, textvariable=mouseoption, values=mouseoptions, state="readonly", width=12)
    mousedropdown.place(x=120,y=20)

    clickoption = tk.StringVar()
    clickoption.set("Single")
    clickoptions = ["Single","Hold"]
    clickdropdown = ttk.Combobox(frame2, textvariable=clickoption, values=clickoptions, state="readonly", width=12)
    clickdropdown.place(x=120,y=50)



    

    startbutton = tk.Button(root, text="Start (shift+b)", bg=colour2, font=("Helvetica", 12), width=23, height=3, command=start)
    startbutton.place(x=20, y=150)

    stopbutton = tk.Button(root, text="Stop (shift+b)", bg=colour2, font=("Helvetica", 12), width=23, height=3, comman=stop, state=(tk.DISABLED))
    stopbutton.place(x=265, y=150)

    hotkeybutton = tk.Button(root, text="Change Hotkey", bg=colour2, font=("Helvetica", 12), width=23, height=3, state=(tk.DISABLED))
    hotkeybutton.place(x=20, y=225)

    applybutton = tk.Button(root, text="Apply Settings", bg=colour2, font=("Helvetica", 12), width=23, height=3, command=apply)
    applybutton.place(x=265, y=225)

    


    root.mainloop()

