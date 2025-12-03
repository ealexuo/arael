# Tool Versions

* Node: 20.11.1
* NPM: 10.2.4

# Layout base
* Code: https://github.com/mui/material-ui/tree/v5.15.14/docs/data/material/getting-started/templates/dashboard
* Live Preview: https://mui.com/material-ui/getting-started/templates/dashboard/

# Naming Convention

* https://dev.to/sathishskdev/part-1-naming-conventions-the-foundation-of-clean-code-51ng

# Http Requests with Axios

We use Axios to fetch data. Axios documentation here: https://axios-http.com/docs/intro



# Steps to set up the project locally

1. Clone the repository
2. CD to the project directory (paperly)
3. run `npm install`


# (For reference only) Commands Executed For Project Setup 

```shell

npx create-react-app paperly --template typescript

npm install @mui/material @emotion/react @emotion/styled
npm install @fontsource/roboto
npm install @mui/icons-material

```


# (For reference only) Internationalization

## Documentation
https://react.i18next.com/latest/using-with-hooks

## Set up

```bash

npm install react-i18next i18next --save

# if you'd like to detect user language and load translation
# this wasn't used by now but will keep the reference
npm install i18next-http-backend i18next-browser-languagedetector --save

```

## Files and folders structured

The folder called 'locales' will contain sub folders with the language code, for example 'en', 'es', etc. Each folder will contain a Json file with the corresponding translation. All the locale files must contain the same json structure.

### i18n.ts file

This file contains the initial configuration of the i18next library and it is referenced in the index.tsx file.

-------------------------------
The text below this line will be removed later.

# Getting Started with Create React App

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app).

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode.\
Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

The page will reload if you make edits.\
You will also see any lint errors in the console.

### `npm test`

Launches the test runner in the interactive watch mode.\
See the section about [running tests](https://facebook.github.io/create-react-app/docs/running-tests) for more information.

### `npm run build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.\
Your app is ready to be deployed!

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.

### `npm run eject`

**Note: this is a one-way operation. Once you `eject`, you can’t go back!**

If you aren’t satisfied with the build tool and configuration choices, you can `eject` at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (webpack, Babel, ESLint, etc) right into your project so you have full control over them. All of the commands except `eject` will still work, but they will point to the copied scripts so you can tweak them. At this point you’re on your own.

You don’t have to ever use `eject`. The curated feature set is suitable for small and middle deployments, and you shouldn’t feel obligated to use this feature. However we understand that this tool wouldn’t be useful if you couldn’t customize it when you are ready for it.

## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).

# Snackbar library

https://notistack.com/getting-started

# Date Picker

https://mui.com/x/react-date-pickers/date-picker/

# Loading bar

https://klendi.github.io/react-top-loading-bar/

