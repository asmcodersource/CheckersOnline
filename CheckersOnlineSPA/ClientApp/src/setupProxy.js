const { createProxyMiddleware } = require('http-proxy-middleware');
const httpProxy = require('http-proxy');
const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:5015';

const webSocketTarget = env.ASPNETCORE_HTTPS_PORT ? `ws://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'ws://localhost:5015';

const context = [
    "/login",
    "/registration",
    "/tokenvalidation",
    "/statistic",
];

module.exports = function (app) {
    const appProxy = createProxyMiddleware(context, {
        proxyTimeout: 10000,
        target: 'http://192.168.0.100:5124',
        secure: false,
        headers: {
            Connection: 'Keep-Alive'
        }
    });

    app.use(appProxy);
};