from tkinter import *
from tkinter import messagebox
from random import choice, randint, shuffle
import pyperclip
import json


# ---------------------------- PASSWORD GENERATOR ------------------------------- #
def generate_password():
    letters = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z']
    numbers = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']
    symbols = ['!', '#', '$', '%', '&', '(', ')', '*', '+']

    password_letters = [choice(letters) for _ in range(randint(8, 10))]
    password_symbols = [choice(symbols) for _ in range(randint(2, 4))]
    password_numbers = [choice(numbers) for _ in range(randint(2, 4))]

    password_list = password_letters + password_symbols + password_numbers
    
    shuffle(password_list)

    password = "".join(password_list)

    password_entry.delete(0, END)
    password_entry.insert(0, password)
    pyperclip.copy(password)

# ---------------------------- SAVE PASSWORD ------------------------------- #
def save():
    website = str(website_entry.get())
    email = str(email_entry.get())
    password = str(password_entry.get())
    new_data = {
        website: {
            'email': email,
            'password': password
        }}
    
    if len(website) == 0 or len(password) == 0:
        messagebox.showinfo(
            title='Oops', 
            message='Please make sure you haven\'t left any fields empty.')
    else:
        try:
            with open('password-manager/data.json', mode='r') as data_file:
                #Read old data
                data = json.load(data_file)
        except FileNotFoundError:
            with open('password-manager/data.json', mode='w') as data_file:
                json.dump(new_data, data_file, indent=4)
        else:
            #Updating old data with new data
            data.update(new_data)
                
            with open('password-manager/data.json', mode='w') as data_file:
                #Save updated data
                json.dump(data, data_file, indent=4)
        finally:    
            website_entry.delete(0, END)
            password_entry.delete(0, END)

# ---------------------------- SEARCH ------------------------------- #
def search():
    website = str(website_entry.get())
    
    try:
        with open('password-manager/data.json', mode='r') as saved_file:
            search_data = json.load(saved_file)
    except FileNotFoundError:
        messagebox.showinfo(title='Error', 
                            message='No Password File Found.')
    else:
        if website in search_data:
            email_results = search_data[website]['email']
            password_results = search_data[website]['password']
            messagebox.showinfo(title=website, message=f'Email: {email_results}\nPassword: {password_results}')
        else:
            messagebox.showinfo(title='Error', 
                                message=f'{website} website not found in file.')

# ---------------------------- UI SETUP ------------------------------- #
window = Tk()
window.title('YourPass')
window.config(padx=50, pady=50)

logo_img = PhotoImage(file='mypass_logo.png')

canvas = Canvas(width=200, height=200)
canvas.create_image(100, 100, image=logo_img)
canvas.grid(column=1,row=0)

website_label = Label(text='Website:')
website_label.grid(column=0, row=1)
website_label.config(padx=0, pady=1)

website_entry = Entry(width=33)
website_entry.grid(column=1, row=1)
website_entry.focus()

search_button = Button(text='Search', width=15, command=search)
search_button.grid(column=2, row=1)

email_label = Label(text='Email/Username:')
email_label.grid(column=0, row=2)
email_label.config(padx=0, pady=1)

email_entry = Entry(width=52)
email_entry.grid(column=1, columnspan=2, row=2)
email_entry.insert(0, 'zuljinzero@gmail.com')

password_label = Label(text='Password:')
password_label.grid(column=0, row=3)
password_label.config(padx=0, pady=1)

password_entry = Entry(width=33)
password_entry.grid(column=1, row=3)

generate_password_button = Button(text='Generate Password', command=generate_password)
generate_password_button.grid(column=2, row=3)

add_button = Button(text='Add', width=44, command=save)
add_button.grid(column=1, columnspan=2, row=4)







window.mainloop()
