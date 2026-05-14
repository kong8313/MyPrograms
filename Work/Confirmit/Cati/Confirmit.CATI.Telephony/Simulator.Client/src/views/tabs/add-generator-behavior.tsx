import * as React from "react";
import * as api from "../../common/api"
import { connect, TypeOfConnect } from "../../store";
import { TextField, Paper, Box, Button, Divider, RadioGroup, FormControlLabel, Radio, FormControl, FormLabel, Grid, Typography } from "@material-ui/core";
import { useHistory } from "react-router-dom";
import CSS from 'csstype';

const storeEnhancer = connect(
  () => ({}),
  {},
);

type Props = TypeOfConnect<typeof storeEnhancer> & {generatorName: string};


const GeneratorBehaviorTab: React.SFC<Props> = (props) => {
      const history = useHistory();
      const [value, setValue] = React.useState("60"); 
      const [owner, setOwner] = React.useState(""); 
      const [filter, setFilter] = React.useState({
        campaignId: undefined as number | undefined,
        agentId: undefined as number | undefined,
        interviewId: undefined as number | undefined,
      });
      const [adding, setAdding] = React.useState(false);
      const [type, setType] = React.useState<"Value"|"Manual">("Value");
      const add = async () => {
        setAdding(true);
        let bf = filter;
        if(bf.campaignId == "" ) delete bf.campaignId;
        if(bf.agentId == "" ) delete bf.agentId;
        if(bf.interviewId == "" ) delete bf.interviewId;

        await api.addGeneratorBehavior(props.generatorName, {id:'asd', type: type, value: value, owner: owner, filter: filter} );
        setAdding(false);
        history.goBack();
      };

      return (
        <Paper>
          <Box p={2}>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Typography variant="h5">{props.generatorName}</Typography>
              </Grid>
              <Grid item xs={12}>
                <FormControl>
                  <FormLabel>Type</FormLabel>
                  <RadioGroup value={type} onChange={(e)=> setType(e.target.value)}>
                    <FormControlLabel value="Value" control={<Radio />} label="Value" />
                    <FormControlLabel value="Manual" control={<Radio />} label="Manual" />
                  </RadioGroup>
                </FormControl>
              </Grid>
              <Grid item xs={12}>
                <TextField style={{width: "60%"} as CSS.Properties} disabled={type!="Value"} label="Return value" value={value} onChange={(e)=> setValue(e.target.value)}/>
              </Grid>
              <Grid item xs={12}>
                <TextField label="Owner" value={owner} onChange={(e)=> setOwner(e.target.value)}/>
              </Grid>
              <Grid item xs={12}>
                <FormLabel>Filter</FormLabel>
              </Grid>
              <Grid item xs={12}>
                <TextField label="Company ID" value={filter.companyId} onChange={(e)=> setFilter({ ...filter, companyId: e.target.value})}/>
              </Grid>
              <Grid item xs={12}>
                <TextField label="Dialer ID" value={filter.dialerId} onChange={(e)=> setFilter({ ...filter, dialerId: e.target.value})}/>
              </Grid>
              <Grid item xs={12}>
                <TextField label="Campaing ID" value={filter.campaignId} onChange={(e)=> setFilter({ ...filter, campaignId: e.target.value})}/>
              </Grid>
              <Grid item xs={12}>
                <TextField label="Agent ID" value={filter.agentId} onChange={(e)=> setFilter({ ...filter, agentId: e.target.value})}/>
              </Grid>
              <Grid item xs={12}>
                <TextField label="Interview ID" value={filter.interviewId} onChange={(e)=> setFilter({ ...filter, interviewId: e.target.value})}/>
              </Grid>
            </Grid>
          </Box>
          <Divider/>
          <Box p={2} display="flex" justifyContent="flex-end">
            <Button variant="text" color="secondary" onClick={() => history.goBack()}>Back</Button>
            <Button variant="contained" color="primary"  disabled={adding} onClick={add}>Add</Button>
          </Box>
        </Paper>);
}

export default storeEnhancer(GeneratorBehaviorTab);