<template>
    <nav class="sidebar" :class="{ open: isOpen }">
        <ul>
            <li>
                <router-link to="/home" @click="handleLinkClick">
                    🏠 Home
                </router-link>
            </li>
            <li>
                <router-link to="/characters" @click="handleLinkClick">
                    🧍 Characters
                </router-link>
            </li>
            <li>
                <div class="admin-toggle" @click="toggleAdminMenu">
                    🛠️ Admin
                    <span class="arrow" :class="{ open: isAdminMenuOpen }">▸</span>
                </div>
                <transition name="submenu">
                    <ul v-if="isAdminMenuOpen" class="admin-submenu">
                        <li>
                            <router-link to="/actions" @click="handleLinkClick">⚔️ Actions</router-link>
                        </li>
                        <li>
                            <router-link to="/ancestries" @click="handleLinkClick">🧬 Ancestries</router-link>
                        </li>
                        <li>
                            <router-link to="/backgrounds" @click="handleLinkClick">📜 Backgrounds</router-link>
                        </li>
                        <li>
                            <router-link to="/classes" @click="handleLinkClick">🧙 Classes</router-link>
                        </li>
                        <li>
                            <router-link to="/passives" @click="handleLinkClick">🧱 Passives</router-link>
                        </li>
                        <li>
                            <router-link to="/traits" @click="handleLinkClick">🧩 Traits</router-link>
                        </li>
                    </ul>
                </transition>
            </li>
        </ul>
    </nav>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useRoute } from 'vue-router';

const props = defineProps<{ isOpen: boolean }>();
const emit = defineEmits<{ (e: 'close'): void }>();

const isAdminMenuOpen = ref(false);

const toggleAdminMenu = () => {
    isAdminMenuOpen.value = !isAdminMenuOpen.value;
};

const handleLinkClick = () => {
    emit('close');
};

const route = useRoute();
watch(() => route.fullPath, () => {
    isAdminMenuOpen.value = false;
});
</script>

<style scoped>
.sidebar {
    width: 220px;
    background-color: #2c2c2c;
    color: white;
    padding: 20px;
    display: flex;
    flex-direction: column;
    transition: transform 0.3s ease-in-out;
    transform: translateX(0);
    box-shadow: 2px 0 6px rgba(0, 0, 0, 0.3);
}

.sidebar ul {
    list-style: none;
    padding: 0;
    margin: 0;
    width: 100%;
}

.sidebar li {
    margin: 6px 0;
}

.sidebar a {
    color: white;
    text-decoration: none;
    font-size: 1.1rem;
    padding: 10px 14px;
    display: block;
    border-radius: 6px;
    transition: background-color 0.2s ease;
}

.sidebar a:hover {
    background-color: #3d3d3d;
}

.admin-toggle {
    cursor: pointer;
    font-size: 1.1rem;
    padding: 10px 14px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    border-radius: 6px;
    color: white;
}

.admin-toggle:hover {
    background-color: #3d3d3d;
}

.arrow {
    margin-left: 8px;
    font-size: 1.4rem;
    color: white;
    transition: transform 0.2s ease;
}

.arrow.open {
    transform: rotate(90deg);
}

.admin-submenu {
    overflow: hidden;
    padding-left: 14px;
    border-left: 2px solid #555;
    margin-top: 6px;
}

.admin-submenu li {
    margin: 4px 0;
}

.admin-submenu a {
    font-size: 1rem;
    padding-left: 20px;
    border-radius: 4px;
}

.admin-submenu a:hover {
    background-color: #444;
}

/* Transition for submenu */
.submenu-enter-active, .submenu-leave-active {
    transition: all 0.3s ease;
}
.submenu-enter-from, .submenu-leave-to {
    max-height: 0;
    opacity: 0;
    transform: scaleY(0.9);
}
.submenu-enter-to, .submenu-leave-from {
    max-height: 500px;
    opacity: 1;
    transform: scaleY(1);
}

/* Mobile responsiveness */
@media (max-width: 768px) {
    .sidebar {
        position: absolute;
        top: 66px;
        left: 0;
        height: calc(100vh - 66px);
        z-index: 1000;
        transform: translateX(-100%);
    }

    .sidebar.open {
        transform: translateX(0);
    }
}
</style>
