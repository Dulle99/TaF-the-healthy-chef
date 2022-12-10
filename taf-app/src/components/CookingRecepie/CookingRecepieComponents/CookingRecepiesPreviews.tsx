import { KeyboardArrowDown } from "@mui/icons-material";
import { Box, Button, Fade, Grid, Menu, MenuItem, Typography } from "@mui/material";
import { Container } from "@mui/system";
import axios from "axios";
import { prependOnceListener } from "process";
import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import ICookingRecepiePreviewProp from "../CookingRecepieInterface/ICookingRecepiePreviewProps";
import ICookingRecepieType from "../CookingRecepieInterface/ICookingRecepieType";
import CookingRecepiePreview from "./CookingRecepiePreview";


function CookingRecepiesPreview(prop: ICookingRecepieType) {
    const pathname = window.location.pathname;
    const [cookingRecepies, setCookngRecepies] = useState<ICookingRecepiePreviewProp[]>([]);
    const [currentUrl, setCurrentUrl] = useState(pathname);
    const [numberOfCookingRecepiesToGet, setnumberOfCookingRecepiesToGet] = useState(1);
    const [selectedSortingOption, setSelectedSortingOption] = useState("1");
    const [typeOfMealTitle, setTypeOfMealTitle] = useState("");
    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);
    const handleClick = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleClickLoadMore = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        loadCookingRecepies(selectedSortingOption, numberOfCookingRecepiesToGet + 5);
        setnumberOfCookingRecepiesToGet(numberOfCookingRecepiesToGet + 5);
    }

    function loadCookingRecepies(sortingType: string, numberOfRecepiesToGet: number) {
        let url: string = "";
        if (sortingType == "1") {
            url = `https://localhost:5001/api/CookingRecepie/GetCookingRecepiesByPopularity/${prop.typeOfMeal}/${numberOfRecepiesToGet}`;
        }
        else if (sortingType == "2") {
            url = `https://localhost:5001/api/CookingRecepie/GetFastestToCookCookingRecepies/${prop.typeOfMeal}/${numberOfRecepiesToGet}`;
        }
        else if (sortingType == "3") {
            url = `https://localhost:5001/api/CookingRecepie/GetCookingRecepiesByPublicationDate/${prop.typeOfMeal}/${numberOfRecepiesToGet}/true`
        }
        else if (sortingType == "4") {
            url = `https://localhost:5001/api/CookingRecepie/GetCookingRecepiesByPublicationDate/${prop.typeOfMeal}/${numberOfRecepiesToGet}/false`
        }

        const fetchCookingRecepies = async () => {
            const result = await axios.get<ICookingRecepiePreviewProp[]>(url);
            setCookngRecepies(result.data);
        }
        fetchCookingRecepies();
    }

    const handleClose: React.MouseEventHandler<HTMLLIElement> = (e) => {
        let selectedSortingOption: string = e.currentTarget.id;
        if (selectedSortingOption === "1" || selectedSortingOption === "2"
            || selectedSortingOption === "3" || selectedSortingOption === "4") {
            loadCookingRecepies(e.currentTarget.id, 5);
            setSelectedSortingOption(e.currentTarget.id);
            setnumberOfCookingRecepiesToGet(5);
        }
        setAnchorEl(null);
    };

    useEffect(() => {
        setnumberOfCookingRecepiesToGet(1);
        setCurrentUrl(pathname);
    }
        , [pathname])

    useEffect(() => {
        const fetchCookingRecepiesByPopularity = async () => {
            console.log(prop.typeOfMeal);
            const result = await axios.get<ICookingRecepiePreviewProp[]>
                (`https://localhost:5001/api/CookingRecepie/GetCookingRecepiesByPopularity/${prop.typeOfMeal}/5`);
            setCookngRecepies(result.data);

            if (prop.typeOfMeal === "Dorucak") {
                setTypeOfMealTitle("doručka");
            }
            else if (prop.typeOfMeal === "Rucak") {
                setTypeOfMealTitle("ručka");
            }
            else if (prop.typeOfMeal === "Vecera") {
                setTypeOfMealTitle("večere");
            } else {
                setTypeOfMealTitle("užine");
            }
        }
        fetchCookingRecepiesByPopularity();
    }, [currentUrl]);

    useEffect(() => {
        document.title = "Pregled recepta"
    }, []);

    return (
        <>
            <main>
                <Container>
                    <Typography component="h1"
                        variant="h2"
                        align="left"
                        color="text.primary">Recepti za spremanje {typeOfMealTitle}
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
                                background: '#92279A', margin: '3px', ':hover': {
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
                            <MenuItem onClick={handleClose} id={"2"}><Typography color='#92279A'>Najbrže za spremanje</Typography></MenuItem>
                            <MenuItem onClick={handleClose} id={"3"}><Typography color='#92279A'>Najnoviji prvo</Typography></MenuItem>
                            <MenuItem onClick={handleClose} id={"4"}><Typography color='#92279A'>Najstariji prvo</Typography></MenuItem>
                        </Menu>
                    </Box>
                    <Grid>
                        {cookingRecepies.map((cookingRecepie) => (<CookingRecepiePreview {...cookingRecepie} key={cookingRecepie.cookingRecepieId} />))}
                    </Grid>
                    <Box style={{ display: "flex", justifyContent: "center" }}>

                        <Button
                            onClick={handleClickLoadMore}
                            size='small' sx={{
                                background: '#92279A', margin: '3px', ':hover': {
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
export default CookingRecepiesPreview;