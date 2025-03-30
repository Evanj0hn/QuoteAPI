import requests
import random

BASE_URL = "https://localhost:7228/api/quoteapi"  # Replace port

# Ignore SSL warning (for local testing only)
import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

def load_quotes_from_file():
    file_path = "bulk_quotes.txt"

    try:
        with open(file_path, "r", encoding="utf-8") as f:
            lines = f.readlines()

        for line in lines:
            if "--" not in line:
                continue

            parts = line.strip().split("--")
            text = parts[0].strip().strip('"')
            author = parts[1].strip() if len(parts) > 1 else "Unknown"

            quote_data = {
                "text": text,
                "author": author,
                "tags": []
            }

            res = requests.post(f"{BASE_URL}", json=quote_data, verify=False)
            if res.status_code == 201:
                print(f"Added: \"{text}\"")
            else:
                print(f"Failed: \"{text}\" | Status: {res.status_code}")

    except FileNotFoundError:
        print("File not found: bulk_quotes.txt")


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
        print("\n1. Load quotes from file")
        print("2. View all quotes")
        print("3. Add a quote")
        print("4. Show a random quote")
        print("5. Exit")

        choice = input("Choose an option: ")
        if choice == "1":
            load_quotes_from_file()
        elif choice == "2":
            quotes = load_quotes()
            for q in quotes:
                print(f'[{q["id"]}] "{q["text"]}" - {q.get("author", "Unknown")} {q["likes"]} likes')
        elif choice == "3":
            add_quote()
        elif choice == "4":
            random_quote()
        elif choice == "5":
            break
        else:
            print("Invalid option.")

if __name__ == "__main__":
    main()
