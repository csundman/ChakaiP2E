import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';
import path from 'path';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [plugin()],
  server: {
    host: '0.0.0.0', // Allow connections from any host on the local network
    port: 50859,
    strictPort: true,
    allowedHosts: [
      'chad-desktop', // Add your machine's hostname here
      'chad-desktop.local', // Add your machine's hostname here
      'localhost',    // Optionally add localhost
      '0.0.0.0',      // Optionally allow any local network devices
      'unlamentable-unscourging-santos.ngrok-free.dev'
    ],
  },
  resolve: {
    alias: {
        '@': path.resolve(__dirname, 'src')
    }
  }
});
