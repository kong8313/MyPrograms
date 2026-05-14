import * as React from "react";
import * as api from '../../common/api'
import { connect, IState, Actions, TypeOfConnect } from "../../store";
import { ActivityInfo } from "../../common/api";
import { Box, Typography, TableContainer, Paper, Table, TableHead, TableRow, TableCell, TableBody, IconButton, Dialog, Button, Snackbar, SnackbarContent, TextField, Input, makeStyles } from "@material-ui/core";
import {withRouter, RouteComponentProps, useHistory} from 'react-router-dom'
import useInterval from 'react-useinterval';

const storeEnhancer = connect(
  (state: IState) => ({url: state.url}),
  {
    setActivities: Actions.setActivities,
  },
);

const useStyles = makeStyles({
  actionParameterInput: {
    padding: '0px',
  },
});

type Props = TypeOfConnect<typeof storeEnhancer>;


const ActivitiesTab: React.SFC<Props & RouteComponentProps> = (props) => {
  const history = useHistory();
  const classes = useStyles();
  const [error, setError] = React.useState(null);
  const [param, setParam] = React.useState("");
  const [state, setState] = React.useState<{
        activities: ActivityInfo[], state: "ok"|"error", error?: string
      }>({
        activities:[], state:"ok"
      });
  
  useInterval( () => (async () => {
      try{
        const activities = await api.getActivities();
        setState({ activities, state:"ok"});
        props.setActivities(activities);
      }
      catch(error){
        setState({ activities:[], state:"error", error: error.toString()});
      }
    })(), 500 );

  const executeCommand = async (id: string, command: string, param: string) => {
    try{
      await api.executeCommand(id, command, param);
      history.go();
    }
    catch(err){
      setError(err);
    }
  };

  switch(state.state){
    case "ok":
      return (<Box>
          <TableContainer component={Paper}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell align="right">Campaign Id</TableCell>
                <TableCell align="right">Agent Id</TableCell>
                <TableCell align="right">Interview Id</TableCell>
                <TableCell align="right">Owner</TableCell>
                <TableCell align="right">Actions
                  <span>(</span>
                    <Input placeholder="parameter" 
                        value={param} className={classes.textEdit}
                        onChange={(e) => setParam(e.target.value)}/>
                  <span>)</span>
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {state.activities.map((activity) => (
                <TableRow key={activity.id}>
                  <TableCell component="th" scope="row">{activity.name}</TableCell>
                  <TableCell align="right">{activity.context.campaignId}</TableCell>
                  <TableCell align="right">{activity.context.agentId}</TableCell>
                  <TableCell align="right">{activity.context.interviewId}</TableCell>
                  <TableCell align="right">{activity.context.owner}</TableCell>
                  <TableCell align="right">{
                    activity.commands.map(command => (
                      <Button key={activity.id + command} variant="contained" 
                          color="primary" 
                          onClick={() => executeCommand(activity.id, command, param)}>
                        {command}
                      </Button>))}
                  </TableCell>
                </TableRow>) 
              )}
            </TableBody>
          </Table>
        </TableContainer>
        <Snackbar open={!!error} autoHideDuration={6000} onClose={() => setError(null)}>
          <SnackbarContent message={error}/>
        </Snackbar>
      </Box>);
    case "error":
    default:
      return (<Box><Typography color="error">{state?.error ?? "Unknown error"}</Typography></Box>)
  }
}

export default withRouter(storeEnhancer(ActivitiesTab));