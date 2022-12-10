import { Button, Grid, TextField } from "@mui/material";
import { useState } from "react";
import PushPopAwardProp from "./PushPopAwardProp";


function AuthorAwardComponent(prop: PushPopAwardProp) {
    const [awardName, setAward] = useState("");
    const [dateOfRecevingAward, setDateOfRecevingAward] = useState<Date>(new Date());

    const handleAwardFieldChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        setAward(e.currentTarget.value);
    }

    const handleDateOfRecevingAwardChange: React.ChangeEventHandler<HTMLInputElement> = (e) => {
        console.log(e.currentTarget.value);
        setDateOfRecevingAward(new Date(e.currentTarget.value));
    }

    const buttonHandlerAddAward = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        prop.pushAwardToArray({ awardName: awardName, dateOfReceivingAward: dateOfRecevingAward })
    }

    const eventHandlerConfig: React.ChangeEventHandler<HTMLInputElement>[] = [handleAwardFieldChange, handleDateOfRecevingAwardChange];
    const Configs: string[][] = [["Naziv nagrade", "NazivNagrade", "nazivNagrade", "text"],
    ["Datum dobijanja nagrade", "DatumDobijanjaNagrade", "datumDobijanjaNagrade", "date"]];
    return (

        <Grid container spacing={2} sx={{ marginLeft: "10px", marginTop: "1px" }}>

            {Configs.map((conf, ind) => (
                <Grid item xs={12} sm={6} key={conf[0]}>
                    <TextField
                        label={conf[0] === "Datum dobijanja nagrade" ? "" : conf[0]}
                        name={conf[1]}
                        id={conf[2]}
                        type={conf[3]}
                        required
                        fullWidth
                        onChange={eventHandlerConfig[ind]}

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
                onClick={buttonHandlerAddAward}
            >
                Dodaj nagradu
            </Button>
        </Grid>

    );
}
export default AuthorAwardComponent;