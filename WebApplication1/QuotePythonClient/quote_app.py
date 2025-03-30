import requests
import random

BASE_URL = "https://localhost:7228/api/quoteapi" # Replace port

# Ignore SSL warning (for local testing only)
import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

def load_quotes():
    response = requests.get(BASE_URL, verify=False)
    if response.status_code == 200:
        return response.json()
    else:
        print("Failed to fetch quotes.")
        return []

def add_quote():
    text = input("Enter quote text: ")
    author = input("Enter author: ")
    tags = input("Enter tags (comma separated): ").split(",")

    quote_data = {
        "text": text,
        "author": author,
        "tags": [t.strip() for t in tags]
    }

    response = requests.post(BASE_URL, json=quote_data, verify=False)
    if response.status_code == 201:
        print("Quote added successfully.")
    else:
        print("Failed to add quote.")
        print("Status code:", response.status_code)
        print("Response:", response.text)


def random_quote():
    quotes = load_quotes()
    if quotes:
        quote = random.choice(quotes)
        print(f'"{quote["text"]}" - {quote.get("author", "Unknown")}')
        print("Tags:", ", ".join(quote["tags"]))

def main():
    while True:
        print("\n1. View all quotes")
        print("2. Add a quote")
        print("3. Show a random quote")
        print("4. Exit")

        choice = input("Choose an option: ")
        if choice == "1":
            quotes = load_quotes()
            for q in quotes:
                print(f'[{q["id"]}] "{q["text"]}" - {q.get("author", "Unknown")} ❤️ {q["likes"]} likes')
        elif choice == "2":
            add_quote()
        elif choice == "3":
            random_quote()
        elif choice == "4":
            break
        else:
            print("Invalid option.")

if __name__ == "__main__":
    main()
