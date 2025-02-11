import tkinter as tk
from tkinter import ttk


# Placeholder for function references
updateValuesFunc = None 
toggleFunc = None


def setFunctions(func1):
    global toggleFunc
    toggleFunc = func1


def getinterval():
    global cps
    try:
        temp = int(cpsEntry.get())
        if(temp > 0 and temp < 51):
            cps = cpsEntry.get()
        else:
            cps = 10
    except:
        cps = 10

    return (1/int(cps))
    

def getmousebutton():
    global mousedropdown
    return mousedropdown.get()
    
def getclicktype():
    global clickdropdown
    return clickdropdown.get()


def getposchoice():
    global poschoice
    return poschoice.get()


def getcords():
    global xEntry
    global yEntry
    return [xEntry.get(), yEntry.get()]


def toggle():
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
    global poschoice
    global xEntry
    global yEntry

    colour1 = "#EEEEEE"
    colour2 = "#DDDDDD"

    
    root = tk.Tk()
    root.title("AutoClicker")
    root.geometry("450x325")
    root.resizable(False, False)
    root.configure(bg=colour1)


    # Create a frame with a border
    frame1 = tk.Frame(root, bd=2, relief="solid")
    frame1.place(x=10, y=20, width=210, height=100)

    frame2 = tk.Frame(root, bd=2, relief="solid")
    frame2.place(x=230, y=20, width=210, height=100)

    frame3 = tk.Frame(root, bd=2, relief="solid")
    frame3.place(x=10, y=140, width=290, height=100)

    frame4 = tk.Frame(root, bd=2, relief="solid")
    frame4.place(x=310, y=140, width=130, height=100)

    #Subtitles
    tk.Label(root, text="Options", bg=colour1, font=("Helvetica", 11)).place(x=30,y=10)
    tk.Label(root, text="Position", bg=colour1, font=("Helvetica", 11)).place(x=250,y=10)
    tk.Label(root, text="CPS", bg=colour1, font=("Helvetica", 11)).place(x=30,y=130)

    image = tk.PhotoImage(file="hamter.png")
    img = tk.Label(frame4, image=image)
    img.image = image
    img.place(x=0,y=0)



    #CPS Options
    def only_numbers(char, entry_value, maxLength):
        return char.isdigit() and len(entry_value) <= maxLength

    tk.Label(frame3, text="Clicks Per Second (CPS):", bg=colour1, font=("Helvetica", 10)).place(x=10,y=20)
    validate1 = root.register(lambda char, entry_value: only_numbers(char, entry_value, 2)), "%S", "%P"
    cpsEntry = tk.Entry(frame3, width=4, validate="key", validatecommand=validate1, borderwidth=1, highlightthickness=1, relief="solid", font=("Helvetica", 10))
    cpsEntry.insert(0,"10")
    cpsEntry.place(x=175,y=20)


    #General Options
    tk.Label(frame1, text="Mouse Button:", bg=colour1, font=("Helvetica", 10)).place(x=10,y=20)
    tk.Label(frame1, text="Click Type:", bg=colour1, font=("Helvetica", 10)).place(x=10,y=50)

    mouseoption = tk.StringVar()
    mouseoption.set("Left Click")
    mouseoptions = ["Left Click","Right Click","Middle Mouse"]
    mousedropdown = ttk.Combobox(frame1, textvariable=mouseoption, values=mouseoptions, state="readonly", width=12)
    mousedropdown.place(x=105,y=20)

    clickoption = tk.StringVar()
    clickoption.set("Single")
    clickoptions = ["Single","Hold"]
    clickdropdown = ttk.Combobox(frame1, textvariable=clickoption, values=clickoptions, state="readonly", width=12)
    clickdropdown.place(x=105,y=50)


    #Mouse Position Options
    poschoice = tk.IntVar()
    poschoice.set(1)

    validate2 = root.register(lambda char, entry_value: only_numbers(char, entry_value, 4)), "%S", "%P"

    currposbox = tk.Radiobutton(frame2, variable=poschoice, value=1)
    currposbox.place(x=10,y=20)
    tk.Label(frame2, text="Current Position", bg=colour1, font=("Helvetica", 10)).place(x=35,y=20)

    cordposbox = tk.Radiobutton(frame2, variable=poschoice, value=2)
    cordposbox.place(x=10,y=50)

    tk.Label(frame2, text="X:", bg=colour1, font=("Helvetica", 10)).place(x=35,y=50)
    xEntry= tk.Entry(frame2, width=4, validate="key", validatecommand=validate2, borderwidth=1, highlightthickness=1, relief="solid", font=("Helvetica", 10))
    xEntry.insert(0,"0")
    xEntry.place(x=55,y=50)

    tk.Label(frame2, text="Y:", bg=colour1, font=("Helvetica", 10)).place(x=95,y=50)
    yEntry= tk.Entry(frame2, width=4, validate="key", validatecommand=validate2, borderwidth=1, highlightthickness=1, relief="solid", font=("Helvetica", 10))
    yEntry.insert(0,"0")
    yEntry.place(x=115,y=50)
    


    #Buttons
    startbutton = tk.Button(root, text="Start (shift+b)", bg=colour2, font=("Helvetica", 11), relief="solid",
                            width=22, height=2, command=toggle, borderwidth=1, highlightthickness=2)
    startbutton.place(x=10, y=260)

    stopbutton = tk.Button(root, text="Stop (shift+b)", bg=colour2, font=("Helvetica", 11), relief="solid",
                            width=22, height=2, command=toggle, borderwidth=1, highlightthickness=2, state=(tk.DISABLED))
    stopbutton.place(x=230, y=260)

    


    root.mainloop()

