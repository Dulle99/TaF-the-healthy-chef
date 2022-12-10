import { createTheme, Theme } from "@mui/material";

function CustomTheme(): Theme{
    const theme = createTheme();

    theme.typography.h3 = {
      fontSize: '1.2rem',
      '@media (min-width:600px)': {
        fontSize: '1.5rem',
      },
      [theme.breakpoints.up('md')]: {
        fontSize: '3rem',
      },
    };
    return theme;
}
export default CustomTheme;