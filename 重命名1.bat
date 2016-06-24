
for /f "delims=" %%a in ('dir /b/s/a-d *.java') do rename "%%a" *.java2
for /f "delims=" %%a in ('dir /b/s/a-d *.cs') do rename "%%a" *.cs2

