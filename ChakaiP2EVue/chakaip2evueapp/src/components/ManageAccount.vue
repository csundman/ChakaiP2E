<template>
    <div class="manage-account">
        <h1>Manage Account</h1>

        <div v-if="account">
            <div class="info-section">
                <p><strong>Username:</strong> {{ account.Username }}</p>
                <button @click="editing = 'username'">Change Username</button>
                <form v-if="editing === 'username'" @submit.prevent="updateUsername">
                    <input v-model="newUsername" placeholder="New username" />
                    <div class="form-buttons">
                        <button type="submit">Save</button>
                        <button type="button" class="cancel-btn" @click="cancelEdit">Cancel</button>
                    </div>
                </form>
            </div>

            <div class="info-section">
                <p><strong>Email:</strong> {{ account.Email }}</p>
                <button @click="editing = 'email'">Change Email</button>
                <form v-if="editing === 'email'" @submit.prevent="updateEmail">
                    <input v-model="newEmail" type="email" placeholder="New email" />
                    <div class="form-buttons">
                        <button type="submit">Save</button>
                        <button type="button" class="cancel-btn" @click="cancelEdit">Cancel</button>
                    </div>
                </form>
            </div>

            <div class="info-section">
                <p><strong>Password:</strong> ********</p>
                <button @click="editing = 'password'">Change Password</button>
                <form v-if="editing === 'password'" @submit.prevent="updatePassword">
                    <input v-model="newPassword" type="password" placeholder="New password" />
                    <input v-model="confirmPassword" type="password" placeholder="Confirm new password" />
                    <div class="form-buttons">
                        <button type="submit">Save</button>
                        <button type="button" class="cancel-btn" @click="cancelEdit">Cancel</button>
                    </div>
                </form>
            </div>

            <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
        </div>
        <div v-else>
            <p>Loading account information...</p>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import { accountStore } from '@/stores';
import api from '@/api';
import { getErrorMessage } from '@/utils/error';

const account = computed(() => accountStore.items[0] || null);

const editing = ref<'username' | 'email' | 'password' | null>(null);
const newUsername = ref('');
const newEmail = ref('');
const currentPassword = ref('');
const newPassword = ref('');
const confirmPassword = ref('');
const errorMessage = ref<string | null>(null);

const cancelEdit = () => {
    editing.value = null;
    newUsername.value = '';
    newEmail.value = '';
    currentPassword.value = '';
    newPassword.value = '';
    confirmPassword.value = '';
    errorMessage.value = null;
};

const updateUsername = async () => {
    if (!account.value || !newUsername.value.trim()) return;
    try {
        const res = await api.put(`/accounts/${account.value.AccountId}`, {
            Username: newUsername.value.trim()
        });
        accountStore.items[0] = res.data.result;
        cancelEdit();
    } catch (err) {
        errorMessage.value = getErrorMessage(err);
    }
};

const updateEmail = async () => {
    if (!account.value || !newEmail.value.trim()) return;
    try {
        const res = await api.put(`/accounts/${account.value.AccountId}`, {
            Email: newEmail.value.trim()
        });
        accountStore.items[0] = res.data.result;
        cancelEdit();
    } catch (err) {
        errorMessage.value = getErrorMessage(err);
    }
};

const updatePassword = async () => {
    if (!account.value || !newPassword.value || newPassword.value !== confirmPassword.value) {
        errorMessage.value = 'Please check your password fields.';
        return;
    }

    try {
        const res = await api.put(`/accounts/${account.value.AccountId}`, {
            Password: newPassword.value
        });
        cancelEdit();
    } catch (err) {
        errorMessage.value = getErrorMessage(err);
    }
};
</script>

<style scoped>
.manage-account {
    max-width: 600px;
    margin: 2rem auto;
    background: #ffffff;
    padding: 24px;
    border-radius: 8px;
    box-shadow: 0 0 12px rgba(0, 0, 0, 0.08);
    color: #333;
    font-family: sans-serif;
}

h1 {
    font-size: 1.8rem;
    margin-bottom: 1.5rem;
    color: #222;
}

.info-section {
    margin-bottom: 28px;
}

p {
    margin: 0 0 8px;
    font-size: 1.1rem;
}

button {
    margin-top: 6px;
    padding: 8px 14px;
    background-color: #4a90e2;
    color: white;
    border: none;
    border-radius: 4px;
    font-size: 0.95rem;
    cursor: pointer;
    transition: background-color 0.2s;
}

button:hover {
    background-color: #357ab7;
}

form {
    margin-top: 10px;
}

input {
    padding: 10px;
    margin-bottom: 10px;
    border-radius: 4px;
    border: 1px solid #ccc;
    font-size: 1rem;
    width: 100%;
}

.form-buttons {
    display: flex;
    gap: 10px;
    margin-top: 4px;
}

.cancel-btn {
    background-color: #e0e0e0;
    color: #333;
}

.cancel-btn:hover {
    background-color: #cfcfcf;
}

.error {
    color: red;
    margin-top: 20px;
}
</style>
