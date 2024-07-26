import { BrowserRouter, Routes, Route } from "react-router-dom";
import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import GameLayout from './components/GamePage/GameLayout'
import BrowserLayout from './components/Main'
import './App.css'

export default function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<BrowserLayout />} />
                <Route path="/gameroom" element={<GameLayout />} />
            </Routes>
        </BrowserRouter>
    );
}

