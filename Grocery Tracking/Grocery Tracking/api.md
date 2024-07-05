# Grocery Tracking API

## Endpoints

### POST /item

Adds a new grocery item.

#### Parameters

- `itemName` (string): The name of the item.
- `price` (decimal): The price of the item.
- `date` (DateTime): The date of the purchase.

#### Response

- 200 OK

---

### GET /item/{itemName}

Gets the price history of a specific item.

#### Parameters

- `itemName` (string): The name of the item.

#### Response

- 200 OK: Returns a list of `Purchase` objects.
- 404 Not Found: If no purchases are found.

---

### GET /item/summary

Gets the monthly spending summary.

#### Response

- 200 OK: Returns a list of `Summary` objects.
- 404 Not Found: If no summaries are found.

---

### POST /item/budget

Sets the budget for a user.

#### Parameters

- `userId` (int): The ID of the user.
- `budget` (decimal): The budget to set.

#### Response

- 200 OK

---

### PUT /item/budget

Updates the budget for a user.

#### Parameters

- `userId` (int): The ID of the user.
- `budget` (decimal): The new budget.

#### Response

- 200 OK

---

### GET /item/{userId}/remainingBudget

Calculates the remaining budget for a user.

#### Parameters

- `userId` (int): The ID of the user.

#### Response

- 200 OK: Returns the remaining budget as a decimal.

---

## Models

### User

- `Id` (int): The ID of the user.
- `Name` (string): The name of the user.
- `Email` (string): The email of the user.
- `PasswordHash` (string): The hashed password of the user.

### Purchase

- `Id` (int): The ID of the purchase.
- `User` (User): The user who made the purchase.
- `ItemName` (string): The name of the item.
- `Price` (decimal): The price of the item.
- `Date` (DateTime): The date of the purchase.

### Summary

- `Month` (string): The month of the summary.
- `TotalSpending` (decimal): The total spending for the month.
