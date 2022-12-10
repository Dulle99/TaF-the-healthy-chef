import { FormControl, FormControlLabel, FormLabel, Grid, Radio, RadioGroup, TextField } from "@mui/material";
import { useState } from "react";
import IReaderProp from "./IReaderProp";

function ReaderAdditionalFields(props: IReaderProp) {

    const [typeOfReader, setTypeOfReader] = useState('Student');

    const handlers: React.ChangeEventHandler<HTMLInputElement>[] = [props.handleSchoolNameChange, props.handleTypeOfStudentChange];
    const handleTypeOfReaderChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setTypeOfReader((event.target as HTMLInputElement).value);
        props.handleTypeOfStudentChange(event);
    }

    return (

        <Grid container >
            <Grid item xs={12} key="nazivSkole">
                <TextField
                    label="Naziv skole"
                    name="NazivSkole"
                    id="nazivSkole"
                    type="text"
                    required
                    fullWidth
                    style={{ marginLeft: "10px" }}
                    onChange={handlers[0]}
                />

                <FormControl style={{ margin: '20px' }}>
                    <FormLabel id="demo-controlled-radio-buttons-group">Vrsta čitaoca</FormLabel>
                    <RadioGroup
                        aria-labelledby="demo-controlled-radio-buttons-group"
                        name="controlled-radio-buttons-group"
                        value={typeOfReader}
                        onChange={handleTypeOfReaderChange}
                        style={{ flexDirection: 'row', margin: '10px' }}

                    >
                        <FormControlLabel value="Student" control={<Radio />} label="Student" />
                        <FormControlLabel value="Srednjoškolac" control={<Radio />} label="Srednjoškolac" />
                    </RadioGroup>
                </FormControl>
            </Grid>

        </Grid>
    );
}
export default ReaderAdditionalFields;