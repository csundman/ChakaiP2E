<template>
    <div v-if="isVisible" class="modal-overlay">

        <div class="modal">
            <h3>{{ props.title }}</h3>

            <div v-if="props.errorMessage" class="error-message">
                {{ props.errorMessage }}
            </div>
            
            <div class="modal-create-form">
                <form @submit.prevent="handleSubmit()">
                    <div v-for="(field, index) in fields" :key="index" class="form-group">
                        <label :for="field.name">{{ field.label }}</label>

                        <!-- Basic Input -->
                        <input
                            v-if="field.type === 'string' || field.type === 'number'"
                            v-model="formData[field.name]"
                            :type="field.type === 'number' ? 'number' : 'text'"
                            :placeholder="field.placeholder"
                            :required="field.required"
                        />

                        <!-- Static Dropdown -->
                        <select
                            v-else-if="field.type === 'select'"
                            v-model="formData[field.name]"
                            :required="field.required"
                        >
                            <option v-for="option in field.options" :key="option" :value="option">
                                {{ option }}
                            </option>
                        </select>

                        <!-- Searchable Dropdown -->
                        <select
                            v-else-if="field.type === 'searchable-select'"
                            v-model="formData[field.name]"
                            :required="field.required"
                        >
                            <option
                                v-for="option in field.options"
                                :key="option.value"
                                :value="option.value"
                            >
                                {{ option.label }}
                            </option>
                        </select>

                        <!-- Multiline Text Area -->
                        <textarea
                            v-else-if="field.type === 'textarea'"
                            v-model="formData[field.name]"
                            :placeholder="field.placeholder"
                            :required="field.required"
                            rows="4"
                        />

                        <!-- Error message -->
                        <div v-if="field.error" class="error-message">{{ field.error }}</div>
                    </div>

                    <!-- Submit & Cancel buttons -->
                    <div class="form-actions">
                        <button type="submit" class="btn save">Submit</button>
                        <button type="button" class="btn cancel" @click="handleCancel()">Cancel</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>


<script setup>
import { ref, watch } from 'vue';

const props = defineProps({
    title: {
        type: String,
        required: true
    },
    fields: {
        type: Array,
        required: true
    },
    onSubmit: {
        type: Function,
        required: true
    },
    onCancel: {
        type: Function,
        required: false
    },
    errorMessage: {
        type: String,
        required: false
    }
});

// Use `isVisiblePropertyName` for controlling visibility with `v-model`
const isVisible = ref(false);
const currentDefaults = ref({});

const open = (defaults = {}) => {
    currentDefaults.value = defaults;
    resetForm();
    isVisible.value = true;
};

const close = () => {
    isVisible.value = false;
    clearForm();
};

defineExpose({ open, close });

const formData = ref({});

props.fields.forEach(field => {
    formData.value[field.name] = field.value ?? '';
});

const handleSubmit = () => {
    props.onSubmit(formData.value);
    if (!props.errorMessage)
        close();
};

// Cancel the modal (close it)
const handleCancel = () => {
    if (props.onCancel) {
        props.onCancel(); // Execute the onCancel callback if it's provided
    }
    close();
};

const resetForm = () => {
    props.fields.forEach(field => {
        formData.value[field.name] = currentDefaults.value[field.name] ?? field.value ?? '';
    });
};

const clearForm = () => {
    props.fields.forEach(field => {
        formData.value[field.name] = field.value ?? '';
    });
};

</script>

<style scoped>
.modal {
    background-color: white;
    padding: 24px;
    border-radius: 12px;
    box-shadow: 0 4px 16px rgba(0, 0, 0, 0.2);
    width: 100%;
    max-width: 500px;
    margin: 0 auto;
    animation: fadeIn 0.3s ease-in-out;
}

h3 {
    margin-bottom: 20px;
    font-size: 1.5rem;
    text-align: center;
    color: #333;
}

.modal-create-form form {
    display: flex;
    flex-direction: column;
    gap: 16px;
}

.form-group {
    display: flex;
    flex-direction: column;
}

label {
    margin-bottom: 6px;
    font-weight: 500;
    color: #555;
}

input,
select {
    padding: 10px 12px;
    border: 1px solid #ccc;
    border-radius: 6px;
    font-size: 1rem;
    transition: border-color 0.2s;
}

input:focus,
select:focus {
    outline: none;
    border-color: #007bff;
}

select {
    appearance: none;
    background-color: #fff;
    background-image: url("data:image/svg+xml;charset=US-ASCII,%3Csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 4 5'%3E%3Cpath fill='%23666' d='M2 0L0 2h4zM2 5L0 3h4z'/%3E%3C/svg%3E");
    background-repeat: no-repeat;
    background-position: right 10px center;
    background-size: 8px 10px;
    padding-right: 30px;
}

select:focus {
    border-color: #007bff;
    box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
}

textarea {
    padding: 10px 12px;
    border: 1px solid #ccc;
    border-radius: 6px;
    font-size: 1rem;
    font-family: inherit;
    resize: vertical;
    transition: border-color 0.2s;
    min-height: 100px;
}

textarea:focus {
    outline: none;
    border-color: #007bff;
    box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
}

.form-actions {
    display: flex;
    justify-content: flex-end;
    gap: 10px;
    margin-top: 20px;
}

/* Fade-in animation */
@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(-10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

@media (prefers-color-scheme: dark) {
    .modal {
        background-color: #2a2a2a;  /* Dark background */
        box-shadow: 0 4px 16px rgba(0, 0, 0, 0.5);  /* Darker shadow */
    }

    h3 {
        color: #e0e0e0;  /* Light text for headings */
    }

    label {
        color: #e0e0e0;  /* Light text for labels */
    }

    input,
    select,
    textarea {
        background-color: #444;  /* Dark background for inputs */
        border: 1px solid #555;  /* Dark border */
        color: #e0e0e0;  /* Light text for inputs */
    }

    input:focus,
    select:focus,
    textarea:focus {
        border-color: #007bff;  /* Focus border color */
        box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25);
    }

    .form-actions {
        margin-top: 20px;
    }
}

</style>
