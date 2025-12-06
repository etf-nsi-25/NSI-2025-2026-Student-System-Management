import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import basicSsl from '@vitejs/plugin-basic-ssl';

// https://vite.dev/config/
export default defineConfig({
  plugins: [react(), basicSsl({
    name: 'localhost',
    domains: ['localhost'],
    certDir: './certs'
  })],
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5283',
        changeOrigin: true,
        secure: false
      }
    }
  },
})
