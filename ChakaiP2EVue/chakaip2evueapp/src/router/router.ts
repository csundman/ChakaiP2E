import { createRouter, createWebHistory } from 'vue-router';
import Login from '../pages/Login.vue'; // path to your Login component
import Home from '../pages/Home.vue'; // path to your Home component
import Characters from '../components/Characters.vue';
import Welcome from '../components/ChakaiWelcome.vue';
import SignUp from '../pages/SignUp.vue';

const routes = [
  {
    path: '/',
    redirect: '/login'
  },
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  {
    path: '/signup',
    name: 'SignUp',
    component: SignUp
  },
  {
    path: '/home',
    component: Home, // Wrap pages with sidebar
    children: [
        { path: '', component: Welcome },
        { path: '/account', component: () => import('@/components/ManageAccount.vue') },
        { path: '/characters', component: Characters },
        { path: '/ancestries', component: () => import('@/components/Ancestries.vue') },
        { path: '/backgrounds', component: () => import('@/components/Backgrounds.vue') },
        { path: '/classes', component: () => import('@/components/Classes.vue') },
        { path: '/actions', component: () => import('@/components/Actions.vue')},
        { path: '/traits', component: () => import('@/components/Traits.vue') },
        { path: '/passives', component: () => import('@/components/Passives.vue') },
        {
          path: '/characters/:characterId',
          name: 'CharacterDetails',
          component: () => import('@/components/CharacterDetails.vue')
        },
        {
          path: '/actions/:actionId',
          name: 'ActionDetails',
          component: () => import('@/components/ActionDetails.vue')
        },
        {
          path: '/passives/:passiveId',
          name: 'PassiveDetails',
          component: () => import('@/components/PassiveDetails.vue')
        }
    ]
}
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

const nonAuthRoutes = ['Login', 'SignUp']; // List of allowed route names

// Add navigation guard to protect private routes
router.beforeEach((to, from, next) => {
  // Check if the user is trying to access a protected route (like /dashboard)
  if (!nonAuthRoutes.includes(to.name as string) && !localStorage.getItem('authToken')) {
    // If not logged in and trying to access a private route, redirect to login
    next('/login');
  } else if (to.name === 'Login' && localStorage.getItem('authToken')) {
    // If already logged in, move to home
    next('/Home');
  } else {
    // Otherwise, allow access to the route
    next();
  }
});

export default router;
