import { useState } from "react";
import { useTranslation } from "react-i18next";
import Page from "../../components/Page";
import { FormGroup, Grid, MenuItem, Select } from "@mui/material";


export default function Languages() {

  const { i18n } = useTranslation();
  const [language, setLanguage] = useState(i18n.language);

  const handleLangChange = (evt: any) => {
    const lang = evt.target.value;
    console.log(lang);
    setLanguage(lang);
    i18n.changeLanguage(lang);
  };

  return (
    <Page title="Languages">
      <Grid container className="center">
        <Grid item justifyContent={"center"} minWidth={50}>
          <FormGroup>            
            <Select
              id="lang-select"
              value={language}
              onChange={handleLangChange}
            >
              <MenuItem value={"en"}>English</MenuItem>
              <MenuItem value={"es"}>Spanish</MenuItem>
            </Select>
          </FormGroup>
        </Grid>
      </Grid>
    </Page>
  );
}
