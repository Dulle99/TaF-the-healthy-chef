import { Box, Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle, Typography, useMediaQuery, useTheme } from "@mui/material";
import { useState } from "react";
import IDeleteSavedContent from "../Interfaces/IDeleteSavedContent";
import { MdDelete } from "react-icons/md";

function DeleteSavedContent(prop: IDeleteSavedContent) {
    const [open, setOpen] = useState(false);
    
    const handleDeleteSavedRecepie = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        setOpen(true);
    }

    const handleClose = () => {
        setOpen(false);
    };

    const handleDigalogAprove = () => {
        prop.removeContentFromReadLater(prop.typeOfContent === "savedCookingRecepies" ? true : false, prop.contentId);
        setOpen(false);
    };

    
    return (<>
        <Box sx={{ display: "flex", flexDirection: "row", justifyContent: "flex-end", mt: 1, mb: 1 }}>
            <Button
                onClick={handleDeleteSavedRecepie}
                size='small' sx={{
                    background: '#f02d0f', margin: '3px', ':hover': {
                        bgcolor: '#f3563e',
                        color: 'FFFFFF',
                    },
                }}
            >
                <MdDelete color="white" size={20}/>
                
            </Button>
            <Dialog
                open={open}
                onClose={handleClose}
                aria-labelledby="responsive-dialog-title"
            >
                <DialogTitle id="responsive-dialog-title">
                    {`Da li sigurno zelite da izbacite ${prop.typeOfContent === "savedCookingRecepies" ? "recept" : "blog"} iz sacuvanih sadrzaja?`}
                </DialogTitle>
                <DialogActions>
                    <Button autoFocus onClick={handleClose}>
                        Ne
                    </Button>
                    <Button onClick={handleDigalogAprove} autoFocus>
                        Da
                    </Button>
                </DialogActions>
            </Dialog>
        </Box>

    </>);
}
export default DeleteSavedContent;