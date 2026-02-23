<template>
  <div class="app-container">
    <!-- Top Bar Component -->
    <TopBar @toggle-sidebar="toggleSidebar" />

    <!-- Main content wrapper -->
    <div class="content-wrapper" :class="{ 'sidebar-open': isSidebarOpen }">
      <Sidebar :is-open="isSidebarOpen" @close="isSidebarOpen = false" />
      <div class="main-content">
        <router-view />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import TopBar from '@/components/layout/Topbar.vue';
import Sidebar from '@/components/layout/Sidebar.vue';
import { ancestryStore, backgroundStore, cclassStore, accountStore, characterStore, chakaischemaStore } from '@/stores/';

const isSidebarOpen = ref(false);

const toggleSidebar = () => {
  isSidebarOpen.value = !isSidebarOpen.value;
};

onMounted(async () => {
  await chakaischemaStore.load();

  if (chakaischemaStore.errorMessage) {
    console.error('Schema load error:', chakaischemaStore.errorMessage);
  }
});
</script>

<style scoped>
.app-container {
  display: flex;
  flex-direction: column;
  width: 100%;
  height: 100dvh;
}

.content-wrapper {
  display: flex;
  height: calc(100dvh - 70px); /* height of top bar */
}

.content-wrapper.sidebar-open {
  margin-left: -200px;
}

.content-wrapper.sidebar-open .sidebar {
  transform: translateX(0);
}

.main-content {
  flex: 1;
  padding: 20px;
  background-color: #f4f4f4;
  overflow-y: auto;
}

@media (max-width: 767px) {
  .main-content {
    padding-top: 16px;
  }
}

/* Dark mode styles */
@media (prefers-color-scheme: dark) {
  .main-content {
    background-color: #1e1e1e;
    color: #e0e0e0;
  }

  /* Optional: tweak top-level containers if they visually clash */
  .app-container {
    background-color: #121212;
  }

  .content-wrapper {
    background-color: #1a1a1a;
  }
}
</style>
