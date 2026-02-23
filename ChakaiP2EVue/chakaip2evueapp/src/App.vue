<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue';

onMounted(() => {
  let lastHeight = window.visualViewport?.height || window.innerHeight;

  const handler = () => {
    if (window === null) return;
    if (window.visualViewport === null) return;
    const newHeight = window.visualViewport.height;

    // If viewport height increased → keyboard likely closed
    if (newHeight > lastHeight) {
      window.scrollTo({ top: 0, behavior: 'instant' });
    }

    lastHeight = newHeight;
  };

  window.visualViewport?.addEventListener('resize', handler);

  onUnmounted(() => {
    window.visualViewport?.removeEventListener('resize', handler);
  });
});

</script>

<template>
  <main>
    <router-view></router-view> <!-- Where the current route component is displayed -->
  </main>
</template>

<style scoped>

main {
    display: flex;
    flex-direction: column; /* Stacks Welcome and Login vertically */
    justify-content: flex-start; /* Vertically centers items within this container */
    align-items: center; /* Horizontally centers items within this container */
    gap: 20px; /* Optional: Adds space between items */
    width: 100%;
    height: 100%;
}

</style>
