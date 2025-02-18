import tkinter as tk
from tkinter import ttk


def updateprogress(value):
    global root
    global progressbar
    progressbar['value'] = value  # Set the progress bar value
    root.update_idletasks()


def starting():
    global progresslabel
    clearerrormessage()
    updateprogress(0)
    progresslabel.config(text="Starting...")


def finished():
    global progresslabel
    updateprogress(100)
    progresslabel.config(text="Finished")


def createerrormessage(message):
    global errormsg
    errormsg.config(text="Error: " + message)

def clearerrormessage():
    global errormsg
    errormsg.config(text="")


def GUI():
    global root
    global progressbar
    global progresslabel
    global errormsg
    
    root = tk.Tk()
    root.title("SudokuSolver")
    root.geometry("300x300")
    root.resizable(False, False)
    root.configure(bg="#EEEEEE")

    frame = tk.Frame(root, relief="solid")
    frame.place(x=10,y=0, width=280, height=220)

    tk.Label(frame, text="Place Mouse in the first sudoku box and select \n it as shown below:",
             font=("Helvetica", 9)).place(x=20,y=10)
    

    image = tk.PhotoImage(file="guide.png")
    img = tk.Label(frame, image=image)
    img.image = image
    img.place(x=80,y=50)

    tk.Label(frame, text="Once in position press shift+n to start and \n keep the box selected",
             font=("Helvetica", 9)).place(x=20,y=180)

    progresslabel = tk.Label(root, text="", font=("Helvetica", 8))
    progresslabel.place(x=50,y=230)

    progressbar = ttk.Progressbar(root, length=200, mode='determinate')
    progressbar.place(x=50, y=250, width=200)

    errormsg = tk.Label(root, text="", fg="#DC143C", font=("Helvetica", 9))
    errormsg.place(x=50, y=275)


    root.mainloop()
