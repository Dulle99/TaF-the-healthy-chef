import { Box, Button, Grid, TextField, Typography } from "@mui/material";
import AuthorAwardComponent from "./AuthorAwardComponent";
import IAuthorProp from "./IAuthorProp";



function AuthorAdditionalFields(prop: IAuthorProp) {

    const buttonHandlerRemoveAward = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        console.log(event.currentTarget.id);
        prop.removeAwardFromArray(parseInt(event.currentTarget.id, 10));
    }

    const handlersForConfig: React.ChangeEventHandler<HTMLInputElement>[] = [prop.handleBiographyChange, prop.handleYearsOfExpirianceChange];
    const configs: string[][] = [["Biografija", "Biografija", "biografija", "text"],
    ["Godine iskustva", "GodineIskustva", "godineIskustva", "number"]];
    return (

        <Grid container spacing={2} >
            {configs.map((conf, ind) => (
                <Grid item xs={12} key={conf[2]}>
                    <TextField
                        label={conf[0]}
                        name={conf[1]}
                        id={conf[2]}
                        type={conf[3]}
                        required
                        fullWidth
                        style={{ marginLeft: "10px" }}
                        rows={5}
                        multiline={conf[2] === "biografija" ? true : false}
                        onChange={handlersForConfig[ind]}
                    />
                </Grid>
            ))}

            <Grid >
                <Typography sx={{ marginLeft: "30px", marginTop: "5px" }} variant="body1" position="relative">Nagrade:</Typography>

                {prop.awards.map((award, ind) => (
                    <Box maxWidth="400px" sx={{ paddingLeft: "25px" }} key={ind}>
                        <Typography > {ind + 1 + ". " + award.awardName + " " + award.dateOfReceivingAward.getDate() + "/" +
                            (award.dateOfReceivingAward.getMonth() + 1).toString() + "/" + award.dateOfReceivingAward.getFullYear()}

                        </Typography>
                        <Box>
                            <Button id={ind.toString()}
                                size="small"
                                variant="contained"
                                sx={{
                                    background: '#e01f2a', ':hover': {
                                        bgcolor: '#ed7c83',
                                        color: 'FFFFFF',
                                    }, marginLeft: "5px"
                                }}
                                onClick={buttonHandlerRemoveAward}>
                                    Obriši
                            </Button>
                        </Box>
                    </Box>))}
            </Grid>
            <AuthorAwardComponent pushAwardToArray={prop.pushAwardToArray} popAwardFromArray={prop.removeAwardFromArray} />
        </Grid>
    );
}
export default AuthorAdditionalFields;