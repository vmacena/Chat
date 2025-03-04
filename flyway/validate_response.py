import json

try:
    with open('response.json', 'r') as f:
        lambda_response = json.load(f)
except FileNotFoundError:
    print("Error: The file 'response.json' was not found.")
    exit(1)
except json.JSONDecodeError:
    print("Error: Failed to decode JSON from 'response.json'.")
    exit(1)

try:
    flyway_response = json.loads(lambda_response['body'])
except KeyError:
    print("Error: The key 'body' was not found in the response.")
    exit(1)
except json.JSONDecodeError:
    print("Error: Failed to decode JSON from the 'body' of the response.")
    exit(1)

if "error" in flyway_response:
    print(flyway_response["error"]["message"])
    exit(1)

print("Success")