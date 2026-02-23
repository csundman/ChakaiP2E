<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import ChakaiWelcome from '../components/ChakaiWelcome.vue';
import Credentials from '../components/Credentials.vue';
import ModalCreateForm from '../components/common/ModalCreateForm.vue';
import api from '@/api';

const router = useRouter();

const goToSignUp = () => {
    router.push({ name: 'SignUp' });
};

// Forgot password modal logic
const forgotError = ref('');
const forgotSuccess = ref('');
const forgotModal = ref();

const forgotFields = ref([
    {
        name: 'Username',
        label: 'Username',
        type: 'string',
        placeholder: 'Enter your username',
        required: true,
        error: ''
    }
]);

const handleForgotButtonClicked = () => {
    forgotModal.value.open();
};

const handleForgotSubmit = async (formData: any) => {
    forgotError.value = '';
    forgotSuccess.value = '';

    try {
        await api.post('/login/forgot', {
            username: formData.Username
        });

        forgotSuccess.value = 'Password reset instructions have been sent.';
        forgotModal.value.close();
    } catch (error) {
        forgotError.value = 'Failed to send password reset instructions.';
        forgotFields.value[0].error = forgotError.value;
        console.error(error);
    }
};
</script>

<template>
    <div class="login-container">
        <ChakaiWelcome />
        <Credentials />

        <div class="auth-buttons">
          <button @click="goToSignUp">Create an Account</button>
          <button @click="handleForgotButtonClicked">Forgot Password?</button>
      </div>

        <!-- Forgot Password Modal -->
        <ModalCreateForm
            ref="forgotModal"
            title="Reset Password"
            :fields="forgotFields"
            :onSubmit="handleForgotSubmit"
        />
    </div>
</template>

<style scoped>

.login-container {
    display: flex;
    flex-direction: column;
    justify-content: flex-start;
    align-items: center;
    gap: 20px;
    width: 100%;
    padding: 20px;
}

button {
    padding: 10px;
    background-color: #4caf50;
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    margin-top: 8px;
}

button:hover {
    background-color: #388e3c;
}

.auth-buttons {
    display: flex;
    gap: 10px;
    flex-wrap: wrap;
    justify-content: center;
    margin-top: 8px;
}
</style>
