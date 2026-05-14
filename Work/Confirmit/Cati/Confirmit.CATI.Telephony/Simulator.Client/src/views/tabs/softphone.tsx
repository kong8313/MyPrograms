import * as React from "react";
import { Box, Typography, Button, Paper, TextField, Grid, Snackbar, Switch, FormControlLabel } from "@material-ui/core";
import * as api from "../../common/api";
import { config } from "../../config";

// Simple Alert component for notifications
const Alert = (props: any) => {
  const { severity, children, ...other } = props;
  const bgColor = severity === 'success' ? '#4caf50' : 
                 severity === 'error' ? '#f44336' : 
                 severity === 'warning' ? '#ff9800' : '#2196f3';
  
  return (
    <Paper 
      style={{ 
        backgroundColor: bgColor, 
        color: 'white', 
        padding: '8px 16px',
        marginBottom: '16px' 
      }} 
      {...other}
    >
      {children}
    </Paper>
  );
};

const SoftphoneTab: React.FC = () => {
  const [status, setStatus] = React.useState<string>('Not logged in');
  const [messages, setMessages] = React.useState<string[]>([]);
  const [isLoggedIn, setIsLoggedIn] = React.useState<boolean>(false);
  const iframeRef = React.useRef<HTMLIFrameElement>(null);
  const messageLogRef = React.useRef<HTMLDivElement>(null);
  
  const [softphoneConfig, setSoftphoneConfig] = React.useState({
    FrontendUrl: "",
    Host: "",
    Password: "",
    Login: ""
  });
  
  const [configLoaded, setConfigLoaded] = React.useState(false);

  // States for URL handling and loading
  // Initialize debouncedUrl with the same default value to prevent initial reload
  const [debouncedUrl, setDebouncedUrl] = React.useState("");
  const [isDebouncing, setIsDebouncing] = React.useState(false);
  const [iframeReady, setIframeReady] = React.useState(false);
  const loadTimeoutRef = React.useRef<number | null>(null);
  
  // Load saved configuration on mount
  React.useEffect(() => {
    const loadSavedConfig = async () => {
      try {
        const generators = await api.getGenerators();
        
        // Find the generator with name "Methods.RegisterAgentSoftphone.Outcome"
        const softphoneGenerator = generators.find(g => g.name === 'Methods.RegisterAgentSoftphone.Outcome');
        
        if (softphoneGenerator && softphoneGenerator.behaviors && softphoneGenerator.behaviors.length > 0) {
          // Get the first behavior (there should only be one for softphone config)
          const behavior = softphoneGenerator.behaviors.find(b => b.owner === 'SoftphoneTab');
          
          if (behavior && behavior.value) {
            const savedConfig = JSON.parse(behavior.value);
            setSoftphoneConfig(savedConfig);
            setDebouncedUrl(savedConfig.FrontendUrl);
            addMessage('Loaded saved softphone configuration');
          }
        }
      } catch (error) {
        console.error('Error loading saved configuration:', error);
        addMessage('Using default softphone configuration');
      } finally {
        setConfigLoaded(true);
      }
    };
    
    loadSavedConfig();
  }, []);
  
  // Apply debounce effect for URL changes
  React.useEffect(() => {
    // If URL changed, set debouncing state
    if (softphoneConfig.FrontendUrl !== debouncedUrl) {
      setIsDebouncing(true);
    }
    
    // Set a timeout to update the debounced URL after 500ms
    const timeoutId = setTimeout(() => {
      setDebouncedUrl(softphoneConfig.FrontendUrl);
      setIsDebouncing(false);
      
      // If we have a URL, set loading state
      if (softphoneConfig.FrontendUrl && softphoneConfig.FrontendUrl.trim() !== '') {
        
        // Set a timeout for 30 seconds
        if (loadTimeoutRef.current) {
          clearTimeout(loadTimeoutRef.current);
        }
      }
    }, 500);
    
    // Clear the timeout if the URL changes again within 500ms
    return () => clearTimeout(timeoutId);
  }, [softphoneConfig.FrontendUrl]);
  
  // Function to determine if we should use the default URL or custom URL
  const getIframeUrl = () => {
    return debouncedUrl;
  };
  
  // Notification state
  const [notification, setNotification] = React.useState({
    open: false,
    message: "",
    severity: "success" as "success" | "error" | "info" | "warning"
  });
  
  // Handle input changes
  const handleInputChange = (field: string) => (event: React.ChangeEvent<HTMLInputElement>) => {
    setSoftphoneConfig({
      ...softphoneConfig,
      [field]: event.target.value
    });
  };
  
  // Handle notification close
  const handleNotificationClose = () => {
    setNotification({
      ...notification,
      open: false
    });
  };
  
  // Save configuration as generator behavior
  const saveAsGeneratorBehavior = async () => {
    try {
      // First, delete any existing softphone configurations
      const generators = await api.getGenerators();
      const softphoneGenerator = generators.find(g => g.name === 'Methods.RegisterAgentSoftphone.Outcome');
      
      if (softphoneGenerator && softphoneGenerator.behaviors) {
        // Delete all existing behaviors with owner "SoftphoneTab"
        for (const behavior of softphoneGenerator.behaviors) {
          if (behavior.owner === 'SoftphoneTab') {
            try {
              await api.deleteGeneratorBehavior('Methods.RegisterAgentSoftphone.Outcome', behavior.id);
              addMessage('Deleted previous softphone configuration');
            } catch (deleteError) {
              console.error('Error deleting previous config:', deleteError);
            }
          }
        }
      }
      
      // Now save the new configuration
      const configValue = JSON.stringify(softphoneConfig);
      await api.addGeneratorBehavior("Methods.RegisterAgentSoftphone.Outcome", {
        id: 'softphone-config',
        type: "Value",
        value: configValue,
        owner: "SoftphoneTab",
        filter: {}
      });
      
      setNotification({
        open: true,
        message: "Softphone configuration saved as generator behavior",
        severity: "success"
      });
      
      // Also update local storage for convenience
      localStorage.setItem("login", softphoneConfig.Login);
      localStorage.setItem("pass", softphoneConfig.Password);
      
    } catch (error) {
      setNotification({
        open: true,
        message: `Error saving configuration: ${error}`,
        severity: "error"
      });
    }
  };

  // Check login status on component mount
  React.useEffect(() => {
    const login = localStorage.getItem('login');
    if (login) {
      const pass = localStorage.getItem('pass');
      setStatus(`Logged in with login: ${login} and pass: ${pass || ''}`);
      setIsLoggedIn(true);
    } else {
      setStatus('Not logged in');
      setIsLoggedIn(false);
    }
  }, []);

  // Handle logout
  const handleLogout = () => {
    if (iframeRef.current && iframeRef.current.contentWindow && iframeReady) {
      const message = { cmd: 'logout' };
      iframeRef.current.contentWindow.postMessage(message, '*');
      addMessage('Sent logout command');
    } else if (!iframeReady) {
      addMessage('Error: Iframe not ready yet');
    }
  };

  // Send "getUser" command
  const handleGetUser = () => {
    if (iframeRef.current && iframeRef.current.contentWindow && iframeReady) {
      const message = { cmd: 'getUser' };
      iframeRef.current.contentWindow.postMessage(message, '*');
      addMessage('Sent getUser command');
    } else if (!iframeReady) {
      addMessage('Error: Iframe not ready yet');
    }
  };

  // Send login credentials
  const handleLogin = () => {
    if (!iframeReady) {
      addMessage('Error: Iframe not ready yet');
      return;
    }
    
    if (iframeRef.current && iframeRef.current.contentWindow) {
      // For demo purposes, you might want to use a dialog or form to get these values
      const login = prompt('Enter login:');
      const pass = prompt('Enter password:');
      
      if (login && pass) {
        const message = {
          cmd: 'login',
          user: login,
          pass: pass
        };
        iframeRef.current.contentWindow.postMessage(message, '*');
        addMessage('Sent login command');
      }
    }
  };

  const addMessage = (message: string) => {
    const timestamp = new Date().toLocaleTimeString();
    setMessages(prevMessages => [...prevMessages, `[${timestamp}] ${message}`]);
  };

  // Autoscroll to bottom when messages change
  React.useEffect(() => {
    if (messageLogRef.current) {
      messageLogRef.current.scrollTop = messageLogRef.current.scrollHeight;
    }
  }, [messages]);

  // Handle iframe load event
  const handleIframeLoad = () => {
    setIframeReady(true);
    addMessage('Iframe loaded and ready');
  };

  const prevIframeUrlRef = React.useRef<string>(debouncedUrl);
  
  // Reset iframe ready state only when the actual iframe URL changes
  React.useEffect(() => {
    if (prevIframeUrlRef.current !== debouncedUrl) {
      setIframeReady(false);
      prevIframeUrlRef.current = debouncedUrl;
    }
  }, [debouncedUrl]);

  // Listen for messages from the iframe
  React.useEffect(() => {
    const handleMessage = (event: MessageEvent) => {
      if (event.data && event.data.type) {
        if (event.data.type === 'event') {
          const eventMessage = event.data.message;
          addMessage(`Received event: ${eventMessage}`);
          
          if (eventMessage === 'loginSuccess' || eventMessage === 'userIsLoggedIn') {
            const login = localStorage.getItem('login');
            const pass = localStorage.getItem('pass');
            setStatus(`Logged in with login: ${login || ''} and pass: ${pass || ''}`);
            setIsLoggedIn(true);
          } else if (eventMessage === 'logoutSuccess' || eventMessage === 'userNotLoggedIn') {
            setStatus('Not logged in');
            setIsLoggedIn(false);
          }
        } else if (event.data.type === 'getUserResult') {
          if (event.data.user) {
            addMessage(`Received getUserResult: User ${event.data.user} with extension ${event.data.message.DefaultExtension}`);
          } else {
            addMessage(`Received getUserResult: ${event.data.message}`);
          }
        }
      }
    };

    window.addEventListener('message', handleMessage);
    return () => window.removeEventListener('message', handleMessage);
  }, []);

  // Set fixed height using vh units
  const contentHeight = '80vh';

  return (
    <Box p={3}>
      {/* Notification */}
      {notification.open && (
        <Alert severity={notification.severity} onClose={handleNotificationClose}>
          {notification.message}
        </Alert>
      )}
      
      <Box display="flex" flexDirection="row" style={{ gap: '20px', height: contentHeight }}>
        {/* Left column - Controls and Status */}
        <Box flex={1} display="flex" flexDirection="column" style={{ height: '100%' }}>
          <Typography variant="h5" gutterBottom>Softphone Simulator</Typography>
          
          {/* Softphone Configuration Form */}
          <Paper elevation={3} style={{ padding: '20px', marginBottom: '20px' }}>
            <Typography variant="h6" gutterBottom>Softphone Configuration</Typography>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label={isDebouncing ? "Frontend URL (loading...)" : "Frontend URL"}
                  value={softphoneConfig.FrontendUrl}
                  onChange={handleInputChange('FrontendUrl')}
                  variant="outlined"
                  size="small"
                  margin="dense"
                  helperText={isDebouncing ? 
                    "Waiting to apply changes..." : 
                    "If provided, this URL will be loaded in the iframe in BBCC as a softphone client. If empty, default softphone simulator url will be used."}
                  InputProps={{
                    endAdornment: isDebouncing ? (
                      <span style={{ display: 'inline-block', width: '20px', textAlign: 'center' }}>⌛</span>
                    ) : null
                  }}
                />
              </Grid>
              <Grid item xs={12}>
                <TextField
                  fullWidth
                  label="Host"
                  value={softphoneConfig.Host}
                  onChange={handleInputChange('Host')}
                  variant="outlined"
                  size="small"
                  margin="dense"
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Login"
                  value={softphoneConfig.Login}
                  onChange={handleInputChange('Login')}
                  variant="outlined"
                  size="small"
                  margin="dense"
                />
              </Grid>
              <Grid item xs={12} sm={6}>
                <TextField
                  fullWidth
                  label="Password"
                  value={softphoneConfig.Password}
                  onChange={handleInputChange('Password')}
                  variant="outlined"
                  size="small"
                  margin="dense"
                  type="text"
                />
              </Grid>
              <Grid item xs={12}>
                <Button 
                  variant="contained" 
                  color="primary" 
                  onClick={saveAsGeneratorBehavior}
                  fullWidth
                >
                  Save as Generator Behavior
                </Button>
              </Grid>
            </Grid>
          </Paper>
          
          {/* Status & Controls */}
          <Paper elevation={3} style={{ padding: '20px', marginBottom: '20px' }}>
            <Typography variant="h6" gutterBottom>Status & Controls</Typography>
            <Typography style={{ marginBottom: '8px' }}>{status}</Typography>
            
            <Box display="flex" style={{marginBottom: '0' }}>
              {!isLoggedIn && (
                <Button variant="contained" color="primary" onClick={handleLogin} style={{ marginRight: '8px' }}>
                  Login
                </Button>
              )}
              {isLoggedIn && (
                <Button variant="contained" color="secondary" onClick={handleLogout} style={{ marginRight: '8px' }}>
                  Logout
                </Button>
              )}
              <Button variant="contained" onClick={handleGetUser} style={{ marginRight: '8px' }}>
                Get User
              </Button>
            </Box>
          </Paper>
          
          {/* Message Log */}
          <Paper elevation={3} style={{ padding: '20px', flexGrow: 1, display: 'flex', flexDirection: 'column', overflow: 'hidden' }}>
            <Typography variant="h6" gutterBottom>Message Log</Typography>
            <div ref={messageLogRef} style={{ flexGrow: 1, overflow: 'auto', padding: '10px', border: '1px solid #eee', borderRadius: '4px' }}>
              <ul style={{ listStyleType: 'none', padding: 0, margin: 0 }}>
                {messages.map((msg, index) => (
                  <li key={index} style={{ marginBottom: '5px', borderBottom: '1px solid #f0f0f0', paddingBottom: '5px' }}>{msg}</li>
                ))}
              </ul>
            </div>
          </Paper>
        </Box>
        
        {/* Right column - Softphone Client */}
        <Box flex={1} display="flex" flexDirection="column" style={{ height: '100%' }}>
          <Typography variant="h5" gutterBottom>Softphone Client</Typography>
          <Paper elevation={3} style={{ padding: '20px', flexGrow: 1, display: 'flex', overflow: 'hidden' }}>
            <iframe
              ref={iframeRef}
              src={debouncedUrl}
              style={{
                width: '100%',
                height: '100%',
                border: 'none'
              }}
              title="Softphone"
              onLoad={handleIframeLoad}
            />
          </Paper>
        </Box>
      </Box>
    </Box>
  );
};

export default SoftphoneTab;
