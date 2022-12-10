import { Button, Typography } from "@mui/material";
import { HeaderButtonProp } from "./header-button-prop";

function HeaderButton(prop: HeaderButtonProp) {
    return (
        <Button size='small' sx={{
            background: '#38B82E', margin: '3px'}} >
            <Typography color='#FFFFFF'>{prop.buttonName}</Typography>
        </Button>
    );
}
export default HeaderButton;