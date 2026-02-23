<template>
    <div class="welcome-container">
      <div class="welcome-message">
        <span class="tea-icon">☕</span>
        <h1>Welcome to Chakai P2E!</h1>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    name: 'ChakaiWelcome'
  };
  </script>
  
  <style scoped>

  .welcome-container {
    width: 100%;
    max-width: 600px;
    background-color: white;
    border-radius: 8px;
    text-align: center;
  }
  
  .welcome-message {
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  
  .welcome-message h1 {
    font-size: 2em;
    font-weight: 600;
    color: #4a90e2;
  }
  
  .tea-icon {
    font-size: 10em;
    color: #4a90e2;
  }

  @media (prefers-color-scheme: dark) {
    .welcome-container {
      background-color: #1e1e1e;
    }

    .welcome-message h1 {
      color: #63b3ed; /* Softer blue for dark mode */
    }

    .tea-icon {
      color: #63b3ed;
    }
  }
  </style>
  