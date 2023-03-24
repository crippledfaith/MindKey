docker build -f "D:\doc\Programming\Visual_Studio_2022\MindKey\MindKey.Server\Dockerfile" --force-rm -t mindkeyserver  --label "com.microsoft.created-by=visual-studio" --label "com.microsoft.visual-studio.project-name=MindKey.Server" "D:\doc\Programming\Visual_Studio_2022\MindKey"
docker tag mindkeyserver:latest thewayman86/mindkeyserver:latest
docker push thewayman86/mindkeyserver:latest
