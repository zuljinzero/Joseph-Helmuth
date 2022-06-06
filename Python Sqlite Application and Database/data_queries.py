import sqlite3
import pandas as pd

# login function that accepts two variables and checks those
# variables against the users table to confirm login information.
def login(username, pswd):
	# Connect to the data.db database
	cnct = sqlite3.connect('data.db')
	# Create a cursor
	crs = cnct.cursor()

	crs.execute("SELECT * FROM users WHERE username = (?) AND password = (?)", (username, pswd))

	nullcheck = crs.fetchone()

	# if there are no results display "incorrect username or password".
	# Otherwise print the username and password.
	if nullcheck == None:
		print("incorrect username or password\n")
		return False
	else:
		print(nullcheck)
		print("")
		return True

	# Close the connection to the database.
	cnct.close()

# add_user function that accepts three variables and inserts those
# variables into the users table.
def add_user(username, password, access_level):
	# Connect to database
	cnct = sqlite3.connect('data.db')
	# Create a cursor
	crs = cnct.cursor()

	crs.execute("INSERT INTO users (username, password, access_level) VALUES (?,?,?)", (username, password, access_level))
	print("Add user successful...\n")

	# Commit changes to the database.
	cnct.commit()
	cnct.close()

# change_password function that accepts two variables. the username 
# varaible is used to search the users table for a user and the pswd
# variable is set to the password for that user.
def change_password(username, pswd):
	# Connect to database
	cnct = sqlite3.connect('data.db')
	# Create a cursor
	crs = cnct.cursor()

	crs.execute("SELECT * FROM users WHERE username = (?)", (pswd, username))
	nullcheck = crs.fetchone()
	
	# if there are no results display "Username not found..."
	# Otherwise make the update and display "Password change successful..."
	if nullcheck == None:
		print("Username not found...\n")
	else:
		crs.execute("UPDATE users SET password = (?) WHERE username = (?)", (pswd, username))
		print("Password change successful...\n")


	cnct.commit()
	cnct.close()

# display_all function that displays all data within the accounts table.
def display_all():
	# Connect to database
	cnct = sqlite3.connect('data.db')
	# Create a cursor
	crs = cnct.cursor()

	crs.execute("SELECT rowid, * FROM accounts")
	items = crs.fetchall()

	for i in items:
		print(i)

	print("")

	cnct.close()

# add_record function that accepts three variables and adds those variables
# as a new record in the accounts table.
def add_record(name, account, amount):
	# Connect to database
	cnct = sqlite3.connect('data.db')
	# Create a cursor
	crs = cnct.cursor()

	crs.execute("INSERT INTO accounts (name, account, amount) VALUES (?,?,?)", (name, account, amount))
	print("Add record successful...\n")

	cnct.commit()
	cnct.close()

# add_file function that accepts an excel file. The contents of the file
# will be inserted as new records into the accounts table.
def add_file(file):
	# Connect to database
	cnct = sqlite3.connect('data.db')
	# Create a cursor
	crs = cnct.cursor()

	# Load the excel file that was passed.
	df = pd.read_excel(file)
	# Create a new dictionary with the data from the loaded file.
	dataset = df.to_dict()

	for x in range(0,11,1):
		crs.execute("INSERT INTO accounts (name, account, amount) VALUES (?,?,?)", (dataset[1][x], dataset[2][x], dataset[3][x]))
		
	print("Add file successful...\n")

	cnct.commit()
	cnct.close()

# search_db function that accepts one variable that is used to search the 
# accounts table for an account number matching the variable.
def search_db(actNum):
	# Connect to database
	cnct = sqlite3.connect('data.db')
	# Create a cursor
	crs = cnct.cursor()

	crs.execute("SELECT * FROM accounts WHERE account = (?)", (actNum,))
	nullcheck = crs.fetchone()

	# if there are no results display "Account not found..."
	# Otherwise display the results.
	if nullcheck == None:
		print("Account not found...\n")
	else:
		print(nullcheck)

	print("")

	cnct.close()

# update_record function that accepts two variables that are used to update
# the amount of funds for the account passed into this function. 
def update_record(actNum, amt):
	# Connect to database
	cnct = sqlite3.connect('data.db')
	# Create a cursor
	crs = cnct.cursor()

	crs.execute("SELECT * FROM accounts WHERE account = (?)", (actNum,))
	nullcheck = crs.fetchone()

	# if there are no results display "Account not found..."
	# Otherwise display "Amount updated successfully..."
	if nullcheck == None:
		print("Account not found...\n")
	else:
		crs.execute("UPDATE accounts SET amount = (?) WHERE account = (?)", (amt, actNum))
		print("Amount updated successfully...\n")

	cnct.commit()
	cnct.close()

# delete_record function that accepts one variable and deletes the record of 
# the account that matches that variable from the accounts table.
def delete_record(actNum):
	# Connect to database
	cnct = sqlite3.connect('data.db')
	# Create a cursor
	crs = cnct.cursor()

	crs.execute("SELECT * FROM accounts WHERE account = (?)", (actNum,))
	nullcheck = crs.fetchone()
	
	# if there are no results display "Account not found..."
	# Otherwise delete the passed account and display a confirmation that it was deleted.
	if nullcheck == None:
		print("Account not found...\n")
	else:
		crs.execute("DELETE FROM accounts WHERE account = (?)", (actNum,))
		printString = "Account " + str(actNum) + " deleted successfully..."
		print(printString)
		print("")

	cnct.commit()
	cnct.close()
