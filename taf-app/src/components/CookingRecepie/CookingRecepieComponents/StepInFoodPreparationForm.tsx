import { Box, Button, Grid, TextField } from "@mui/material";
import { useState } from "react";
import IPushStepInArray from "../CookingRecepieInterface/IPushStepInArray";


function StepInFoodPreparationForm(prop: IPushStepInArray){
    const [ordinalNumber, setOrdinalNumber] = useState<string>("1");
    const [stepDescription, setStepDescription] = useState("");
    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        if(ordinalNumber != "" && parseInt(ordinalNumber) != NaN && stepDescription != ""){
            prop.pushStepInFoodPreparationToArray({ordinalNumberOfStep: parseInt(ordinalNumber), stepDescription: stepDescription});
            setOrdinalNumber((parseInt(ordinalNumber) + 1).toString());
            setStepDescription("");
        }
    }
    const handleOrdinalNumberChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        const numberValue = parseInt(e.currentTarget.value);
        if(numberValue != NaN){
            setOrdinalNumber(e.currentTarget.value);
        }
        else{
            setOrdinalNumber("");
        }
    }
    const handleStepDescriptionChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setStepDescription(e.currentTarget.value);
    }
    const handlersConfig: React.ChangeEventHandler<HTMLInputElement>[] = [handleOrdinalNumberChange, handleStepDescriptionChange]
    const configs: string[][] = [["Redni broj koraka", "Redni broj koraka", "redniBrojKoraka", "number"],
    ["Opis koraka", "Opis koraka", "opisKoraka", "text"]];
    return(
        <>
        <Box component="form" noValidate onSubmit={handleSubmit} sx={{ mt: 3 }}>
                    <Grid container spacing={2}>
                    {configs.map((conf, ind) => (
                            <Grid item xs={12} sm={6} key={conf[0]}>
                                <TextField
                                    label={conf[0]}
                                    name={conf[1]}
                                    id={conf[2]}
                                    type={conf[3]}
                                    required
                                    fullWidth
                                    autoFocus={conf[1] === "Ime" ? true : false}
                                    onChange={handlersConfig[ind]}
                                    value={conf[3]=="number" ? ordinalNumber : stepDescription}
                                />
                            </Grid>
                        ))}
                        <Button
                size="small"
                type="submit"

                variant="contained"
                sx={{
                    mt: 3, mb: 2, background: '#2f9a27', ':hover': {
                        bgcolor: '#27699A',
                        color: 'FFFFFF',
                    },
                    marginLeft: "25px"
                }}
            >
                Dodaj korak
            </Button>
                    </Grid>

                    </Box>
        </>
    );
}
export default StepInFoodPreparationForm;