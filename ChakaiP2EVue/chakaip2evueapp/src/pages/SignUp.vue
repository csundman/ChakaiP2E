<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import api from '../api.ts';
import { getErrorMessage } from '../utils/error.ts'

// Get the router instance
const router = useRouter();

const newAccount = ref({
    username: '',
    email: '',
    password: '',
    confirmPassword: '',
    registrationKey: '' // Added registration key
});

const submitForm = async () => {
    console.log("submit clicked");

    // Reset the error message before submitting
  errorMessage.value = '';

    if (newAccount.value.password !== newAccount.value.confirmPassword) {
        errorMessage.value = 'Passwords do not match!';
        return;
    }

    // Destructure and remove confirmPassword before sending to the API
    const { confirmPassword, ...newAccountForAPI } = newAccount.value;

    // Call the API to create the account
    try {
        const response = await api.post("/accounts", newAccountForAPI);
        // If successful, show the success message in a modal
        modalMessage.value = response.data.message; // Assuming the API returns a message field
        showModal.value = true; // Show the modal
        resetForm(); // Optionally reset the form
    } catch (error) {
        console.error("Error creating account:", error);
        // Handle error, possibly show an alert with error message
        errorMessage.value = 'Could not create Account: ' + getErrorMessage(error);
    }

};

const resetForm = () => {
    newAccount.value = { username: '', email: '', password: '', confirmPassword: '', registrationKey: '' };
}

// Go back to the login page when the button is clicked
const goBackToLogin = () => {
  router.push('/login');
};

const modalMessage = ref(''); // Holds the success message for the modal
const showModal = ref(false); // Controls the visibility of the modal
const errorMessage = ref(''); // Holds error message to show on the page

</script>

<template>
  <div class="sign-up-container">
    <h2>Create New Account</h2>
    <form @submit.prevent="submitForm">
      <label for="username">Username:</label>
      <input v-model="newAccount.username" type="text" id="username" required />

      <label for="email">Email (Optional):</label>
      <input v-model="newAccount.email" type="email" id="email" />

      <label for="password">Password:</label>
      <input v-model="newAccount.password" type="password" id="password" required />

      <label for="confirmPassword">Confirm Password:</label>
      <input v-model="newAccount.confirmPassword" type="password" id="confirmPassword" required />

      <label for="registrationKey">Registration Key:</label>
      <input v-model="newAccount.registrationKey" type="text" id="registrationKey" required />

      <button type="submit">Create Account</button>
    </form>

    <!-- Show error message if an error occurred -->
    <div v-if="errorMessage" class="error-message">
      <p>{{ errorMessage }}</p>
    </div>

    <!-- Button to go back to the login page -->
    <button @click="goBackToLogin" class="back-to-login-btn">Back to Login</button>

    <!-- Modal to show success message -->
    <div v-if="showModal" class="modal">
      <div class="modal-content">
        <h3>{{ modalMessage }}</h3>
        <button @click="goBackToLogin">Return to Login</button>
      </div>
    </div>

  </div>
</template>

<style scoped>
.sign-up-container {
    background: white;
    padding: 20px;
    border-radius: 10px;
    box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
    width: 100%;
    max-width: 400px;
    margin: auto;
}

h2 {
    text-align: center;
    margin-bottom: 20px;
}

form {
    display: flex;
    flex-direction: column;
}

label {
    margin-bottom: 8px;
    font-weight: bold;
}

input {
    margin-bottom: 15px;
    padding: 10px;
    font-size: 1em;
    border: 1px solid #ccc;
    border-radius: 5px;
}

button {
    padding: 10px;
    font-size: 1em;
    border: none;
    border-radius: 5px;
    cursor: pointer;
    background-color: #4caf50;
    color: white;
}

button:hover {
    background-color: #388e3c;
}

.back-to-login-btn {
  margin-top: 20px;
  background-color: #f44336; /* Red color */
  color: white;
}

.back-to-login-btn:hover {
  background-color: #d32f2f; /* Darker red on hover */
}

/* Modal Styles */
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: rgba(0, 0, 0, 0.5); /* Semi-transparent background */
}

.modal-content {
  background-color: white;
  padding: 20px;
  border-radius: 10px;
  text-align: center;
  width: 300px;
}

.modal-content button {
  margin-top: 20px;
  background-color: #4caf50;
}

.modal-content button:hover {
  background-color: #388e3c;
}

/* Error message styles */
.error-message {
  color: red;
  margin-top: 20px;
  font-size: 1em;
}

@media (prefers-color-scheme: dark) {
    .sign-up-container {
        background: #1e1e1e;
        color: #f5f5f5;
        box-shadow: 0px 4px 10px rgba(255, 255, 255, 0.05);
    }

    label {
        color: #e0e0e0;
    }

    input {
        background-color: #2a2a2a;
        color: #f0f0f0;
        border: 1px solid #555;
    }

    button {
        background-color: #4caf50;
        color: white;
    }

    button:hover {
        background-color: #388e3c;
    }

    .back-to-login-btn {
        background-color: #f44336;
    }

    .back-to-login-btn:hover {
        background-color: #d32f2f;
    }

    .modal-content {
        background-color: #2a2a2a;
        color: #ffffff;
        box-shadow: 0 0 10px rgba(255, 255, 255, 0.05);
    }

    .error-message {
        color: #ff6b6b;
    }
}

</style>
