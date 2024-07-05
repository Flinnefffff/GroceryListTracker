<div align="center">
  <img src="https://vcdn-giadinh.vnecdn.net/2017/10/18/tet-2311-1508322246.jpg" alt="Groceries List Tracker Logo" width="300px">
</div>


# Groceries List Tracker 🛒📊

Welcome to the Groceries List Tracker, your ultimate companion for managing groceries, tracking prices, and analyzing budgets! 🚀

## Core Functionalities 🌟

- **Item & Price Tracking**: Keep track of items purchased and their prices organized by date or time period (e.g., weekly, monthly).

- **Price Comparison**: Visualize price changes over time, highlighting increases and decreases through interactive graphs and charts.

- **Budget & Spending Analysis**: Set and monitor your grocery budget, with alerts if it's exceeded to help you stay on track financially.

## Windows Application (C#) 🖥️
 ...

### User Interface 🎨

Experience a user-friendly interface with:
- Intuitive forms for adding grocery items and prices.
- Dynamic data visualizations using state-of-the-art libraries.
- Clear spending summaries and budget progress visualizations.

### Communication with Backend 📡

Effortlessly communicate with the backend using C#'s powerful networking capabilities to make necessary requests to the REST API.

## Database (MariaDB) 🗃️
- **Username**: e2101065
- **Password**: DZCtWC5pEC2

### Table Design 📊

- **items**: item_id, item_name, category, description
- **purchases**: purchase_id, item_id, price, quantity, purchase_date
- **users**: user_id, username, password, email
- **user_purchases**: user_id, purchase_id

## Additional Features (for Web/Mobile Expansion) 🚀

- **User Accounts**: Enable users to save their data and track trends over longer periods.
- **Recipe Integration**: Generate shopping lists and estimated costs based on user-inputted recipes.
- **Store Price Comparisons**: Help users find the best deals by comparing prices from different stores.
- **Alerts & Notifications**: Notify users of significant price changes or when they are nearing their budget limit.

### Considerations 🤔

- **Scalability**: Consider cloud-hosted databases for easier scaling as your user base grows.
- **Security**: Implement robust user authentication and data encryption to protect sensitive financial information.
- **Usability**: Prioritize an intuitive and user-friendly interface for optimal user adoption.

## Author Information 📝

- **Name**: Nguyen An Phuong Linh.
- **Teammate**: https://github.com/NguyenHuy190303, https://github.com/e2101089.

---
Feel free to reach out for any inquiries or collaborations! Let's make grocery tracking fun and easy! 🥳
