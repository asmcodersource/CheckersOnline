import { Main } from "./components/Main";
import { GameLayout } from "./components/GamePage/GameLayout";

const AppRoutes = [
  {
    index: true,
    element: <Main />
    },
    {
        path: 'gameroom',
        element: <GameLayout />
    }
];

export default AppRoutes;
