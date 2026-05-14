import * as React from 'react'
import { SFC } from 'react'
import {routes} from './tabs'
import AppBar from '@material-ui/core/AppBar';
import Box from '@material-ui/core/Box';
import RefreshIcon from '@material-ui/icons/Refresh';
import DoneIcon from '@material-ui/icons/Done';
import { Toolbar, IconButton, Typography, InputBase, makeStyles, fade, Badge, Link} from '@material-ui/core';
import { connect, Actions, TypeOfConnect } from '../store';
import { IState } from '../store/store';
import {Route, withRouter, RouteComponentProps, Redirect, NavLink, Switch} from 'react-router-dom';
import {config} from "./../config";

const useStyles = makeStyles((theme) => ({
  search: {
    position: 'relative',
    borderRadius: theme.shape.borderRadius,
    backgroundColor: fade(theme.palette.common.white, 0.15),
    '&:hover': {
      backgroundColor: fade(theme.palette.common.white, 0.25),
    },
    marginLeft: 0,
    width: '100%',
    [theme.breakpoints.up('sm')]: {
      marginLeft: theme.spacing(1),
      width: 'auto',
    },
  },
  searchIcon: {
    padding: theme.spacing(0, 2),
    height: '100%',
    position: 'absolute',
    pointerEvents: 'none',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'center',
  },
  inputRoot: {
    color: 'inherit',
  },
  inputInput: {
    padding: theme.spacing(1, 1, 1, 0),
    // vertical padding + font size from searchIcon
    paddingLeft: `calc(1em + ${theme.spacing(4)}px)`,
    transition: theme.transitions.create('width'),
    width: '100%',
    [theme.breakpoints.up('sm')]: {
      width: '20ch',
      '&:focus': {
        width: '40ch',
      },
    },
  },
  navLink:{
    display: 'flex',
    textTransform: 'uppercase',
    color: 'inherit',
    padding: '6px 12px',
    opacity: '0.7',
    fontFamily: '"Roboto", "Helvetica", "Arial", sans-serif',
    fontSize: '0.875rem',
    fontWeight: 500,
    textDecoration: 'none',
    borderBottom: '2px'
  },
  selectedNavLink: {
    opacity: '1',
    borderBottom: `2px solid ${theme.palette.secondary.main}`,
  }
}));
const storeEnhancer = connect(
  (state: IState) => ({url: state.url, activities : state.activities}),
  {
    configure: Actions.configure
  },
);

type Props = TypeOfConnect<typeof storeEnhancer>;

const App: SFC<Props & RouteComponentProps> = (props) => {
  const [url, setUrl] = React.useState(props.url);

  const classes = useStyles();
  return (
    <div>
      <AppBar position="static">
        <Toolbar>
          <NavLink to={routes.activities.link()} className={classes.navLink} activeClassName={classes.selectedNavLink}>
            {routes.activities.name}
            <Badge badgeContent={props.activities.length} color="secondary">
            </Badge>
          </NavLink>
          <NavLink to={routes.generators.link()} className={classes.navLink} activeClassName={classes.selectedNavLink}>
            {routes.generators.name}
          </NavLink>
          <NavLink to={routes.agents.link()} className={classes.navLink} activeClassName={classes.selectedNavLink}>
            {routes.agents.name}
          </NavLink>
          <NavLink to={routes.campaigns.link()} className={classes.navLink} activeClassName={classes.selectedNavLink}>
            {routes.campaigns.name}
          </NavLink>
          <NavLink to={routes.dialers.link()} className={classes.navLink} activeClassName={classes.selectedNavLink}>
            {routes.dialers.name}
          </NavLink>
          <NavLink to={routes.inbound.link()} className={classes.navLink} activeClassName={classes.selectedNavLink}>
            {routes.inbound.name}
          </NavLink>
          <NavLink to={routes.transfers.link()} className={classes.navLink} activeClassName={classes.selectedNavLink}>
            {routes.transfers.name}
          </NavLink>
          <NavLink to={routes.softphone.link()} className={classes.navLink} activeClassName={classes.selectedNavLink}>
            {routes.softphone.name}
          </NavLink>
          <div style={{flexGrow:1}} />
          <Link href={"/" + config.serviceUrl + "swagger/ui/index"} className={classes.navLink} rel="noopener noreferrer" target="Swagger UI">Swagger</Link>
          <div className={classes.search}>
            <div className={classes.searchIcon}>
              <Typography>Url</Typography>
            </div>
            <InputBase
              placeholder="Simulator address…"
              classes={{
                root: classes.inputRoot,
                input: classes.inputInput,
              }}
              inputProps={{ 'aria-label': 'search' }}
              value={url}
              onChange={(e) => setUrl(e.target.value)}
            />
          </div>
          <Box display={(url != props.url) ? "inline" : "none"}>
            <IconButton color="inherit" onClick={() => props.configure(url)}><DoneIcon/></IconButton>
          </Box>
          <IconButton color="inherit" onClick={() => props.history.push(props.history.location.pathname)}><RefreshIcon/></IconButton>
        </Toolbar>
      </AppBar>
      <Box p={4}> 
        <Switch>
          {
            //!refresh && tabs.map( (tab, i) => <Box m={2} key={i} display={(i == tabIndex) ? "block" : "none"} >{tab.content}</Box>)
            Object.values(routes).filter(route => route.path && route.component).map( (route) => <Route exact key={route.path} path={route.path} component={route.component}/>)
          }
        </Switch>
      </Box>
    </div>
  )
}

export default withRouter(storeEnhancer(App));
