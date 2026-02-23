<template>
    <div class="credentials-container">
      <div class="login-form">
        <h1 class="login-title">Login</h1>
        <form @submit.prevent="loginUser">
          <div class="input-group">
            <label for="username">Username</label>
            <input
              v-model="username"
              type="text"
              id="username"
              placeholder="Enter your username"
              required
            />
          </div>
          <div class="input-group">
            <label for="password">Password</label>
            <input
              v-model="password"
              type="password"
              id="password"
              placeholder="Enter your password"
              required
            />
          </div>
          <button type="submit" class="login-btn">Login</button>
        </form>
        <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>
      </div>
    </div>
  </template>
  
  <script>
  import api from '../api.ts';
  import { chakaischemaStore } from '@/stores/';
  
  export default {
    name: 'Credentials',

    data() {
      return {
        username: '',
        password: '',
        errorMessage: ''
      };
    },
    methods: {
      async loginUser() {
        try {
          const response = await api.post('/login', {
            username: this.username,
            password: this.password
          });
  
          if (response.status === 200) {
            // Handle success (e.g., save token, redirect)
            console.log('Login successful:', response.data);
            localStorage.setItem('authToken', response.data.token);
            await chakaischemaStore.forceLoad();
            this.$router.push('/home');  // Example: redirect to a dashboard page
          }
        } catch (error) {
          this.errorMessage = error || 'Something went wrong!';
        }
      }
    }
  };
  </script>
  
  <style scoped>
  .credentials-container {
    width: 90%;
    max-width: 400px;
    padding: 20px;
    background-color: white;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    border-radius: 8px;
  }
  
  .login-form {
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  
  .login-title {
    font-size: 2em;
    font-weight: 600;
    margin-bottom: 20px;
    color: #4a90e2;
  }
  
  .input-group {
    width: 100%;
    margin-bottom: 15px;
  }
  
  .input-group label {
    display: block;
    font-size: 0.9em;
    margin-bottom: 5px;
    color: #555;
  }
  
  .input-group input {
    width: 100%;
    padding: 10px;
    font-size: 1em;
    border: 1px solid #ddd;
    border-radius: 4px;
    outline: none;
    transition: border-color 0.3s;
  }
  
  .input-group input:focus {
    border-color: #4a90e2;
  }
  
  .login-btn {
    width: 100%;
    padding: 12px;
    background-color: #4a90e2;
    color: white;
    border: none;
    border-radius: 4px;
    font-size: 1em;
    cursor: pointer;
    transition: background-color 0.3s;
  }
  
  .login-btn:hover {
    background-color: #357ab7;
  }
  
  .error-message {
    color: red;
    margin-top: 15px;
    font-size: 0.9em;
    text-align: center;
  }

  @media (prefers-color-scheme: dark) {
    .credentials-container {
        background-color: #2c2c2c; /* Higher contrast than #1e1e1e */
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.6); /* Stronger shadow */
    }

    .login-title {
        color: #80bfff;
    }

    .input-group label {
        color: #ccc;
    }

    .input-group input {
        background-color: #2a2a2a;
        border: 1px solid #444;
        color: #f0f0f0;
    }

    .input-group input:focus {
        border-color: #80bfff;
    }

    .login-btn {
        background-color: #4a90e2;
        color: white;
    }

    .login-btn:hover {
        background-color: #357ab7;
    }

    .error-message {
        color: #ff6b6b;
    }
}
  </style>
  