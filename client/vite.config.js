import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import mkcert from 'vite-plugin-mkcert'
import { fileURLToPath, URL } from 'url'

const target = `http://95.47.167.113:5124`

const context = [
    "/login",
    "/registration",
    "/tokenvalidation",
    "/statistic",
];

const webSocketContext = [
    "/requestbrowsersocket",
    "/requestgamesocket",
    "/requestChatSocket",
];

const proxy = context.reduce((acc, path) => {
    acc[path] = { target, secure: false }
    return acc;
}, {});

webSocketContext.forEach(path => {
    proxy[path] = {
    target: target.replace('http', 'http'),
    ws: true,
    changeOrigin: true,
    secure: false
    };
});

export default defineConfig({
    plugins: [react(), mkcert()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy,
        https: false, 
        port: 44463,
    }
})
