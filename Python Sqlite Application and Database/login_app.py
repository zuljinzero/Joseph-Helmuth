import data_queries

# login function that displays a login menu.
def login():
	print("___LOGIN_MENU___")
	print("1: Login")
	print("2: Enter new user")
	print("3: Change password")
	print("0: Quit")

	# prompts the user to enter a number.
	choice = input("Enter a number: ")

	# if the user enters 1 then they are prompted to enter a username
	# and password. Once entered, the login function in the data_quiries
	# python file is called while passing userName and password.
	if choice == "1":
		userName = input("Enter username: ")
		password = input("Enter password: ")
		# if the username and password are correct call the menu function.
		# if they are not found call the login fucntion again, starting the menu over.
		if data_queries.login(userName, password):
			menu()
		else:
			login()

	# if the user enters 2 then they are prompted to enter a username, password, 
	# and access level. Once entered, the add_user function in the data_quiries
	# python file is called while passing userName, password, and access_level.
	elif choice == "2":
		userName = input("Enter username: ")
		password = input("Enter password: ")
		access_level = input("Enter access level (1-10): ")
		data_queries.add_user(userName, password, access_level)
		login()

	# if the user enters 3 then they are prompted to enter a username
	# and password. Once entered, the change_password function in the data_quiries
	# python file is called while passing userName, and password.
	elif choice == "3":
		userName = input("Enter username: ")
		password = input("Enter new password: ")
		data_queries.change_password(userName, password)
		login()

	# if the user enters 0 then this application closes.	
	elif choice == "0":
		print("Application is now closing...")
		exit()

	# if the user enters a number that is not 1, 2, 3, or 0,
	# they are informed their choice is not one of the options and the
	# login function is called, returning to login menu selection.
	else:
		print("Your choice is not one of the options.\n")
		login()
	
def menu():
	print("___MAIN_MENU___")
	print("1: Display all")
	print("2: Add record")
	print("3: Add from file")
	print("4: Search database")
	print("5: Update amount")
	print("6: Delete record")
	print("0: Logout")

	choice = input("Enter a number: ")
	
	# if the user enters 1 then the display_all function in the data_quiries
	# python file is called.
	if choice == "1":
		data_queries.display_all()
		menu()

	# if the user enters 2 then they are prompted to enter a name, 
	# account number, and amount. Once entered, the add_record function in the data_quiries
	# python file is called while passing name, account, and amount.
	elif choice == "2":
		name = input("Enter name: ")
		account = input("Enter account number: ")
		amount = input("Enter opening amount: ")
		data_queries.add_record(name, account, amount)
		menu()

	# if the user enters 3 then they are prompted to enter a filename 
	# with extension. Once entered, the add_file function in the data_quiries
	# python file is called while passing file.
	elif choice == "3":
		file = input("Enter file name with extension: ")
		data_queries.add_file(file)
		menu()

	# if the user enters 4 then they are prompted to enter account number.
	# Once entered, the search_db function in the data_quiries
	# python file is called while passing actNum.
	elif choice == "4":
		actNum = input("Enter account number: ")
		data_queries.search_db(actNum)
		menu()

	# if the user enters 5 then they are prompted to enter an account number 
	# and amount. Once entered, the update_record function in the data_quiries
	# python file is called while passing account and amount.
	elif choice == "5":
		account = input("Enter account number to update: ")
		amount = input("Enter new amount: ")
		data_queries.update_record(account, amount)
		menu()

	# if the user enters 6 then they are prompted to enter an account number.
	# Once entered, the delete_record function in the data_quiries
	# python file is called while passing account.
	elif choice == "6":
		account = input("Enter account number to delete: ")
		data_queries.delete_record(account)
		menu()

	# if the user enters 0 then the login function is called, exiting the menu fucntion.
	elif choice == "0":
		print("Loging out...\n")
		login()

	# if the user enters a number that is not 1, 2, 3, 4, 5, 6, or 0,
	# they are informed their choice is not one of the options and the
	# menu function is called, returning to menu selection.
	else:
		print("Your choice is not one of the options.\n")
		menu()

login()

