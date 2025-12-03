import { createTheme } from "@mui/material";

const defaultTheme = createTheme({
  palette: {
    mode: 'light'    
  },
  components: {
    MuiDialogActions: {
      styleOverrides: {
        root: {
          margin: '8px'          
        },
      },
    },
    MuiTab:{
      styleOverrides: {
        root: {
          textTransform: 'none',
          fontSize: '16px'
        },
      },
    }
  }
});

export default defaultTheme;