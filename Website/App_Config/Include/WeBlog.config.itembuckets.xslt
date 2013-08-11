<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <xsl:output method="xml" indent="yes"/>

  <!-- Remove news mover event handler -->
  <xsl:template match="/configuration/sitecore/events/event[@name='item:saved']/handler[@type='Sitecore.Sharedsource.Tasks.NewsMover, Sitecore.Sharedsource.NewsMover']"/>

  <xsl:template match="/configuration/sitecore/settings">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
      <setting name="BucketConfiguration.DynamicBucketFolderPath">
        <patch:attribute name="value">Sitecore.Modules.WeBlog.Buckets.DynamicBucketFolderPathSelector, Sitecore.Modules.WeBlog</patch:attribute>
      </setting>
    </xsl:copy>
  </xsl:template>
  
  <!-- Include configuration for DynamicBucketFolderPathSelector -->
  <xsl:template match="/configuration/sitecore">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
      <WeBlog>
        <DynamicBucketFolderPathSelector>
          <default>
            <includeTemplates>
              <blogHome>$(BlogTemplateID)</blogHome>
              <!--<blogEntry>$(EntryTemplateID</blogEntry>-->
            </includeTemplates>
            <xsl:comment>All paths must be lower case</xsl:comment>
            <!--<paths>
              <path>/sitecore/content/home/bucket</path>
            </paths>-->
            <handler type="Sitecore.Modules.WeBlog.Buckets.SimpleCreationDateBucketFolderPath,Sitecore.Modules.WeBlog"/>
          </default>
        </DynamicBucketFolderPathSelector>
      </WeBlog>
    </xsl:copy>
  </xsl:template>
  
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
