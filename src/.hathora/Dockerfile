############################################################################
# This Dockerfile is auto-generated by HathoraDocker.cs @ 2023-06-15_143557
############################################################################

FROM ubuntu

# Copy the server build files into the container, if Dockerfile is @ parent
COPY ./Build-Server .

RUN chmod +x ./Hathora-Unity-LinuxServer.x86_64

# Run the Linux server in headless mode as a dedicated server
CMD ./Hathora-Unity-LinuxServer.x86_64 -mode server -batchmode -nographics
