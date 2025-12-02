import { useEffect } from 'react';
import { Terminal } from 'primereact/terminal';
import { TerminalService } from 'primereact/terminalservice';
export default function RootTerminal() {


    function handleLogin() {
        //navigate
    }

    function handleRegister() {
        TerminalService.emit('response', 'Register flow not implemented yet...');
    }

    useEffect(() => {
        const commandHandler = (text: string) => {
            const input = text.trim();
            if (!input) return;

            const [cmdRaw] = input.split(' ');
            const cmd = cmdRaw.toLowerCase();

            switch (cmd) {
                case 'l':
                    handleLogin();
                    break;
                case 'r':
                    handleRegister();
                    break;
                case 'clear':
                    TerminalService.emit('clear');
                    break;
                default:
                    TerminalService.emit('response', `Unknown command: ${cmdRaw}`);
                    break;
            }
        };

        TerminalService.on('command', commandHandler);

        return () => {
            TerminalService.off('command', commandHandler);
        };
    }, []);

    return (
        <Terminal
            welcomeMessage="Entering Root.Access... Press 'L' to Log in, 'R' to Register a new account..."
            prompt="⟩ _"
            pt={{
                root: { className: 'bg-gray-900 text-white border-round p-4' },
                prompt: { className: 'text-gray-400 mr-2' },
                command: { className: 'text-primary-300' },
                response: { className: 'text-primary-300 whitespace-pre-wrap' }
            }}
        />
    );
}

