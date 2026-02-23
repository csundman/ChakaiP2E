# chakaip2evueapp

This template should help get you started developing with Vue 3 in Vite.

## Recommended IDE Setup

[VSCode](https://code.visualstudio.com/) + [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (and disable Vetur).

## Visual Studio 2022 (F5)

This project is a Visual Studio JavaScript project (`.esproj`). Visual Studio needs a debug profile to show browser targets (e.g. "localhost (Edge)").

- Use the debug target dropdown to select **`localhost (Edge)`** (defined in `launch.vs.json`)
- If the target doesn’t appear, open **Debug → `chakaip2evueapp` Debug Properties** once and set:
  - Start command: `npm run dev`
  - App URL: `http://localhost:50859/`

## Type Support for `.vue` Imports in TS

TypeScript cannot handle type information for `.vue` imports by default, so we replace the `tsc` CLI with `vue-tsc` for type checking. In editors, we need [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) to make the TypeScript language service aware of `.vue` types.

## Customize configuration

See [Vite Configuration Reference](https://vite.dev/config/).

## Project Setup

```sh
npm install
```

### Compile and Hot-Reload for Development

```sh
npm run dev
```

### Type-Check, Compile and Minify for Production

```sh
npm run build
```

### Lint with [ESLint](https://eslint.org/)

```sh
npm run lint
```
