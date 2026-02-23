<template>
    <div v-if="isVisible" class="modal-overlay">
        <div class="modal">
            <h3>{{ message }}</h3>

            <div class="modal-actions">
                <button class="btn delete" @click="handleConfirm">{{ confirmButtonText }}</button>
                <button class="btn cancel" @click="handleCancel">Cancel</button>
            </div>
        </div>
    </div>
</template>

<script setup>
import { ref, watch } from 'vue';

// Props for modal content and actions
const props = defineProps({
    message: {
        type: String,
        required: true
    },
    confirmButtonText: {
        type: String,
        required: true
    },
    onConfirm: {
        type: Function,
        required: true
    },
    onCancel: {
        type: Function,
        required: false
    }
});

// Use `isVisiblePropertyName` for controlling visibility with `v-model`
const isVisible = ref(false);

const open = () => {
    isVisible.value = true;
};

const close = () => {
    isVisible.value = false;
};

defineExpose({ open, close });

// Cancel the modal (close it)
const handleCancel = () => {
    if (props.onCancel) {
        props.onCancel(); // Execute the onCancel callback if it's provided
    }
    close();
};

// Handle the confirmation action
const handleConfirm = () => {
    props.onConfirm();
    close();
};
</script>

<style scoped>
.modal {
    background-color: white;
    padding: 24px;
    border-radius: 8px;
    width: 100%;
    max-width: 400px;
    text-align: center;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.modal-actions {
    margin-top: 20px;
    display: flex;
    justify-content: space-evenly;
}

@media (prefers-color-scheme: dark) {
    .modal {
        background-color: #333;  /* Dark background for the modal */
        box-shadow: 0 4px 8px rgba(255, 255, 255, 0.2);  /* Lighter shadow for contrast */
        color: #e0e0e0;  /* Light text for better readability */
    }

    .modal-actions {
        margin-top: 20px;
    }
}
</style>
