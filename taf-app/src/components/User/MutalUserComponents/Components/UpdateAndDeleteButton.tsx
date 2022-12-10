import { Box, Button, Dialog, DialogActions, DialogTitle, Typography } from "@mui/material";
import { useState } from "react";
import { Link } from "react-router-dom";
import IUpdateAndDeleteButtonProp from "../Interfaces/IUpdateAndDeleteButtonProps";
import { MdDelete } from "react-icons/md";
import { BsFillPencilFill } from "react-icons/bs";


function UpdateAndDeleteButton(prop: IUpdateAndDeleteButtonProp) {
    const [open, setOpen] = useState(false);

    const handleDeleteContent = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        setOpen(true);
    }

    const handleClose = () => {
        setOpen(false);
    };

    const handleDigalogAprove = () => {
        prop.deleteContent(prop.typeOfContent === "cookingRecepies" ? true : false, prop.contentId);
        setOpen(false);
    };

    return (
        <>
            <Box sx={{ display: "flex", flexDirection: "row", justifyContent: "flex-end", mt: 1, mb: 1 }}>
                {prop.typeOfContent === "blogs" ?
                    <Link to='/updateBlog' state={{blogId: prop.contentId,  blogUpdate: true }} style={{ textDecoration: 'none' }}>
                        <Button
                            size='small' sx={{
                                background: '#ebb315', margin: '3px', ':hover': {
                                    bgcolor: '#f1c959',
                                    color: 'FFFFFF',
                                },
                            }}
                        >
                            <BsFillPencilFill color="white" size={20} />
                        </Button>
                    </Link>
                     :
                    <Link to='/updateCookingRecepie' state={{cookingRecepieId: prop.contentId,  cookingRecepieUpdate: true }} style={{ textDecoration: 'none' }}>
                        <Button
                            size='small' sx={{
                                background: '#ebb315', margin: '3px', ':hover': {
                                    bgcolor: '#f1c959',
                                    color: 'FFFFFF',
                                },
                            }}
                        >
                            <BsFillPencilFill color="white" size={20} />
                        </Button>
                    </Link>
                }

                <Button
                    onClick={handleDeleteContent}
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
                        {`Da li sigurno zelite da obrisete ${prop.typeOfContent === "cookingRecepies" ? "recept" : "blog"}?`}
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

        </>
    );
}
export default UpdateAndDeleteButton;