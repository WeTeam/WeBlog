<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/configuration/sitecore/contentSearch/configuration/indexes/index[@id='WeBlog-master' or @id='WeBlog-web']/configuration">
	<xsl:copy>
      <xsl:apply-templates select="@*"/>
      <documentOptions ref="contentSearch/indexConfigurations/defaultWeBlogLuceneIndexConfiguration/documentOptions">
        <xsl:apply-templates select="*"/>
      </documentOptions>
	</xsl:copy>
  </xsl:template>
  
  <xsl:template match="//include/@hint">
	<xsl:attribute name="hint">list:AddIncludedTemplate</xsl:attribute>
  </xsl:template>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>
