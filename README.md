# CheckersOnline

CheckersOnline is a web application for playing checkers, built with ASP.NET Core and React. The application supports playing against a bot with a simple game engine and a Min-Max algorithm, or against another player. Users can create game rooms, connect to games, and enjoy in-room chat and an interactive game board.
The project is quite unstable, since there was a breakdown of ASP.net as a new technology for me.


![game_browser](https://github.com/user-attachments/assets/9213f2f3-dfcd-4d78-a3de-21cf3d10bfde)
![game with player](https://github.com/user-attachments/assets/6539b27f-4433-4a6d-a41a-35a121536d2e)


## Features

- **Play against a bot**: The bot is powered by a simple game engine and uses the Min-Max algorithm for decision-making.
- **Play against other players**: Connect with other players and challenge them to a game of checkers.
- **Create game rooms**: Set up and manage game rooms for playing with friends or other users.
- **Chat functionality**: Communicate with other players within game rooms.
- **Interactive game board**: Play on a dynamic game board with real-time updates.

## Prerequisites

- **MySQL**: You need a MySQL database server running. Set up a new database and update the connection string in `Program.cs` accordingly.
- **.NET SDK**: Ensure you have the .NET SDK installed to restore and run the server-side application.
- **Node.js**: Required for managing and running the client-side application.

## Setup Instructions

### Running the Application Locally

To set up and run CheckersOnline on your local machine, follow these steps:
1. **Install MySQL**: Ensure that MySQL is installed and running on your machine.
2. **Create a Database**: Set up a new MySQL database for the application.
3. **Update Connection String**: Modify the connection string in `Program.cs` to point to your MySQL database.
4. **Change the request proxy address**: For the React application in the vite.config.js file.
