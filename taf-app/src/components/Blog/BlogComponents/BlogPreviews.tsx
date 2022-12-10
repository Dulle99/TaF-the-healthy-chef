import { KeyboardArrowDown } from "@mui/icons-material";
import { Box, Button, Fade, Grid, Menu, MenuItem, Typography } from "@mui/material";
import { Container } from "@mui/system";
import axios from "axios";
import React, { useEffect, useState } from "react";
import IBlogPreviewProp from "../BlogInterfaces/IBlogPreviewProp";
import BlogPreview from "./BlogPreview";

function BlogPreviews() {
    const [blogs, setBlogs] = useState<IBlogPreviewProp[]>([]);
    const [numberOfBlogsToGet, setnumberOfBlogsToGet] = useState(5);
    const [selectedSortingOption, setSelectedSortingOption] = useState("1");
    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);
    const handleClick = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    function loadBlogs(sortingType: string, numberOfBlogsToGet: number){
        let url:string="";
        if(sortingType == "1"){
            url = `https://localhost:5001/api/Blog/GetBlogsByPopularity/${numberOfBlogsToGet}`;
        }
        else if(sortingType == "2"){
            url = `https://localhost:5001/api/Blog/GetBlogsByPublicationDate/${numberOfBlogsToGet}/true`
        }
        else if(sortingType == "3"){
            url = `https://localhost:5001/api/Blog/GetBlogsByPublicationDate/${numberOfBlogsToGet}/false`
        }

        const fetchCookingRecepiesByPopularity = async () => {
            const result = await axios.get<IBlogPreviewProp[]>(url);
            setBlogs(result.data);
        }
        fetchCookingRecepiesByPopularity();
    }

    const handleClose: React.MouseEventHandler<HTMLLIElement> = (e) => {
        
        const selectedSortingOption: string = e.currentTarget.id;
        if(selectedSortingOption === "1" || selectedSortingOption === "2" || selectedSortingOption === "3"){
        setSelectedSortingOption(selectedSortingOption);
        loadBlogs(selectedSortingOption, 5);
    }
    setAnchorEl(null);
    };

    const handleClickLoadMore= (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        loadBlogs(selectedSortingOption, numberOfBlogsToGet+5);
        setnumberOfBlogsToGet(numberOfBlogsToGet+5);
    }

    useEffect(() => {
        const fetchBlogsByPopularity = async () => {
            const result = await axios.get<IBlogPreviewProp[]>
            (`https://localhost:5001/api/Blog/GetBlogsByPopularity/5`);
            setBlogs(result.data);
        }
        fetchBlogsByPopularity();
    }, []);

    useEffect(() => {
        document.title = "Pregled blogova";  
      }, []);
    
    return (
        <>
        
            <main>
                <Container>
                    <Typography component="h1"
                        variant="h2"
                        align="left"
                        color="text.primary">Blogovi
                    </Typography>
                    <Box style={{ display: "flex", justifyContent: "flex-end" }}>

                        <Button
                            id="fade-button"
                            aria-controls={open ? 'fade-menu' : undefined}
                            aria-haspopup="true"
                            aria-expanded={open ? 'true' : undefined}
                            onClick={handleClick}
                            endIcon={<KeyboardArrowDown style={{ color: "#FFFFFF" }} />}
                            size='small' sx={{
                                background: '#92279A', margin: '3px',':hover': {
                                    bgcolor: '#b731c1',
                                    color: 'FFFFFF',
                                },
                            }}
                        >
                            <Typography color='#FFFFFF'>Sortiranje</Typography>
                        </Button>
                        <Menu
                            id="fade-menu"
                            MenuListProps={{
                                'aria-labelledby': 'fade-button',
                            }}
                            anchorEl={anchorEl}
                            open={open}
                            onClose={handleClose}
                            TransitionComponent={Fade}
                        >
                            <MenuItem onClick={handleClose} id={"1"}><Typography color='#92279A'>Po popularnosti</Typography></MenuItem>
                            <MenuItem onClick={handleClose} id={"2"}><Typography color='#92279A'>Najnoviji prvo</Typography></MenuItem>
                            <MenuItem onClick={handleClose} id={"3"}><Typography color='#92279A'>Najstariji prvo</Typography></MenuItem>
                        </Menu>
                    </Box>
                    <Grid>
                        {blogs.map((blog) => (<BlogPreview {...blog} key={blog.blogId} />))}
                    </Grid>
                    <Box style={{ display: "flex", justifyContent: "center" }}>

                        <Button
                            onClick={handleClickLoadMore}
                            size='small' sx={{
                                background: '#92279A', margin: '3px',':hover': {
                                    bgcolor: '#b731c1',
                                    color: 'FFFFFF',
                                },
                            }}
                        >
                            <Typography color='#FFFFFF'>Učitaj još</Typography>
                        </Button>
                        </Box>
                </Container>

            </main>
        </>

    );
}
export default BlogPreviews;