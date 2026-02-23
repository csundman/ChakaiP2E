<template>
    <header class="topbar">
        <div class="top-bar-content">
            <button class="menu-btn" @click="$emit('toggle-sidebar')">☰ Menu</button>

            <div class="account-wrapper" @click="toggleDropdown">
                <div class="account-btn">
                    <span class="user-icon">👤</span>
                    <span class="account-name">Account</span>
                    <span class="dropdown-icon">▾</span>
                </div>
                <transition name="dropdown">
                    <ul v-if="isDropdownOpen" class="dropdown-menu">
                        <li @click="goToManage">Manage</li>
                        <li @click="logout">Logout</li>
                    </ul>
                </transition>
            </div>
        </div>
    </header>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue';
import { useRouter } from 'vue-router';
import { accountStore, actionStore, ancestryStore, backgroundStore, cclassStore, characterStore } from '@/stores';

const router = useRouter();
const isDropdownOpen = ref(false);

const toggleDropdown = () => {
    isDropdownOpen.value = !isDropdownOpen.value;
};

const closeDropdown = () => {
    setTimeout(() => {
        isDropdownOpen.value = false;
    }, 100);
};

const handleClickOutside = (event: MouseEvent) => {
    const dropdown = document.querySelector('.account-wrapper');
    if (dropdown && !dropdown.contains(event.target as Node)) {
        isDropdownOpen.value = false;
    }
};

onMounted(() => {
    document.addEventListener('click', handleClickOutside);
});

onBeforeUnmount(() => {
    document.removeEventListener('click', handleClickOutside);
});

// Reset stores to their initial state
const resetStores = () => {
    ancestryStore.reset();
    backgroundStore.reset();
    cclassStore.reset();
    characterStore.reset();
    accountStore.reset();
    actionStore.reset();
};

const logout = () => {
    localStorage.removeItem('authToken');

    // Reset all stores
    resetStores();

    router.push('/');
};

const goToManage = () => {
    isDropdownOpen.value = false;
    setTimeout(() => {
        router.push('/account');
    }, 100);
};

</script>

<style scoped>
.topbar {
    background-color: #4a90e2;
    padding: 10px;
    color: white;
    display: flex;
    flex-direction: column;
}

.top-bar-content {
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
}

.menu-btn {
    background-color: transparent;
    border: none;
    color: white;
    font-size: 1.5em;
    cursor: pointer;
    padding: 8px;
}

.account-wrapper {
    position: relative;
    outline: none;
}

.account-btn {
    color: white;
    display: flex;
    align-items: center;
    gap: 6px;
    background-color: rgba(255, 255, 255, 0.15);
    padding: 8px 14px;
    border-radius: 6px;
    cursor: pointer;
    user-select: none;
    transition: background-color 0.2s ease;
}

.account-btn:hover {
    background-color: rgba(255, 255, 255, 0.25);
}

.user-icon {
    font-size: 1.2em;
}

.account-name {
    font-size: 1em;
    font-weight: 500;
}

.dropdown-icon {
    font-size: 0.9em;
    transform: translateY(1px);
}

.dropdown-menu {
    position: absolute;
    top: 120%;
    right: 0;
    background-color: #2f2f2f;
    color: white;
    border-radius: 6px;
    box-shadow: 0 4px 10px rgba(0, 0, 0, 0.4);
    padding: 6px 0;
    margin: 0;
    list-style: none;
    min-width: 140px;
    z-index: 10;
}

.dropdown-menu li {
    padding: 10px 16px;
    cursor: pointer;
    transition: background-color 0.2s ease;
}

.dropdown-menu li:hover {
    background-color: #444;
}

/* Dropdown animation */
.dropdown-enter-active,
.dropdown-leave-active {
    transition: all 0.2s ease;
}
.dropdown-enter-from,
.dropdown-leave-to {
    opacity: 0;
    transform: translateY(-5px);
}
.dropdown-enter-to,
.dropdown-leave-from {
    opacity: 1;
    transform: translateY(0);
}

/* Mobile styles */
@media (max-width: 767px) {
    .top-bar-content {
        flex-direction: row;
    }

    .menu-btn {
        align-self: flex-start;
    }

    .account-wrapper {
        align-self: flex-end;
    }
}
</style>
