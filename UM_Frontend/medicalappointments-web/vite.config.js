import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    host: '127.0.0.1',
    port: 5173,
    strictPort: true,
    proxy: {
      '/backend/security': {
        target: 'http://127.0.0.1:5261',
        changeOrigin: true,
        rewrite: path => path.replace(/^\/backend\/security/, ''),
      },
      '/backend/patients': {
        target: 'http://127.0.0.1:5259',
        changeOrigin: true,
        rewrite: path => path.replace(/^\/backend\/patients/, ''),
      },
      '/backend/agenda': {
        target: 'http://127.0.0.1:5260',
        changeOrigin: true,
        rewrite: path => path.replace(/^\/backend\/agenda/, ''),
      },
      '/backend/clinical-care': {
        target: 'http://127.0.0.1:5262',
        changeOrigin: true,
        rewrite: path => path.replace(/^\/backend\/clinical-care/, ''),
      },
    },
  },
});
